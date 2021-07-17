using Assets.KLib.Source.Events;
using Assets.Source._Infrastructure.Pooling;
using Assets.Source._Infrastructure.Spawnable;
using Assets.Source.Cameras;
using Assets.Source.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source.Balls {
    class BallSpawner : MonoBehaviour, IEventListener<GameOverEvent> {

        [SerializeField] private int maxSpawnedBallsAtOnce = 6;
        [SerializeField] private List<BallSpawnerStage> stages;

        private IEnumerator<BallSpawnerStage> currentStageEnumerator;
        private BallSpawnerStage currentStage;
        private List<BallFacade> spawnedBalls;
        private List<BallInitialParameters> ballsAwaitingForSpawn;
        private PoolManager poolManager;
        private Bounds spawnArea;
        private float spawnTimer;
        private float nextStageTime, playTime;
        bool gameOver;

        private BallInitialParameters.Builder BallBuilder => new BallInitialParameters.Builder();

        [Inject]
        private void Init(CameraManager cameraManager, PoolManager poolManager) {
            this.ListenToEvent<GameOverEvent>();
            this.poolManager = poolManager;

            spawnArea = cameraManager.CalculateCameraBounds();
            currentStageEnumerator = stages.GetEnumerator();
            currentStageEnumerator.MoveNext();
            currentStage = currentStageEnumerator.Current;
            nextStageTime = currentStage.EndTime;
            spawnedBalls = new List<BallFacade>();
            ballsAwaitingForSpawn = new List<BallInitialParameters>();
            spawnTimer = 0.5f;
        }

        public void Despawn(BallFacade ballFacade) {
            spawnedBalls.Remove(ballFacade);
            ballFacade.gameObject.Despawn();
        }

        public void OnEvent(GameOverEvent @event) {
            if (!gameOver) {
                StopAllCoroutines();
                ballsAwaitingForSpawn.Clear();
                gameOver = true;
            }
        }

        private void Update() {
            if (gameOver)
                return;

            if ((spawnTimer -= Time.deltaTime) < 0) {
                Spawn();
                spawnTimer = Random.Range(currentStage.MinTimeBetweenSpawns, currentStage.MaxTimeBetweenSpawns);
            }

            if ((playTime += Time.deltaTime) >= nextStageTime) {
                if (currentStageEnumerator.MoveNext()) {
                    currentStage = currentStageEnumerator.Current;
                    nextStageTime = currentStage.EndTime;
                } else {
                    nextStageTime = float.MaxValue;
                }
            }
        }

        private void Spawn() {
            var bombsToSpawnCount = Random.Range(0, currentStage.BombPoolMaxSize + 1);
            var normalBallsToSpawnCount = Random.Range(currentStage.BallPoolMinSize, currentStage.BallPoolMaxSize + 1);

            var bombsToSpawn = CreateBalls(bombsToSpawnCount);
            var normalBallsToSpawn = CreateBalls(normalBallsToSpawnCount);

            StartCoroutine(SpawnBalls(spawnBombs: true, bombsToSpawn));
            StartCoroutine(SpawnBalls(spawnBombs: false, normalBallsToSpawn));
        }

        private List<BallInitialParameters> CreateBalls(int count) {
            var acceptedBalls = new List<BallInitialParameters>();

            for (int i = 0; i < count; i++) {
                if (FindSpawnPosition(out var position, out var diameter)) {
                    var ball = BallBuilder
                        .SetDiffuseTime(currentStage.DefuseMinTime, currentStage.DefuseMaxTime)
                        .SetSpawnPosition(position)
                        .SetDiameter(diameter)
                        .Build();

                    ballsAwaitingForSpawn.Add(ball);
                    acceptedBalls.Add(ball);
                }
            }

            return acceptedBalls;
        }

        IEnumerator SpawnBalls(bool spawnBombs, IEnumerable<BallInitialParameters> ballInitialParameters) {
            if (spawnBombs)
                yield return new WaitForSeconds(Random.Range(0f, 0.25f));

            var spawnableType = spawnBombs ? SpawnableType.Ball_Bomb : SpawnableType.Ball_Normal;

            foreach (var ballParameter in ballInitialParameters) {
                bool posOk = true;
                foreach (var ball in spawnedBalls) {
                    if (Vector2.Distance(ballParameter.SpawnPosition, ball.transform.position) < (ballParameter.Diameter + ball.transform.localScale.x) / 2f + 0.25f) {
                        posOk = false;
                        break;
                    }
                }

                if (posOk) {
                    poolManager.Spawn(spawnableType, (Vector3)ballParameter.SpawnPosition + new Vector3(0, 0, 5), Vector3.zero, ballParameter.Diameter * Vector3.one, out var spawnedBallGo);
                    var spawnedBall = spawnedBallGo.GetComponent<BallFacade>();
                    spawnedBall.OnSpawn(ballParameter);
                    spawnedBalls.Add(spawnedBall);
                    ballsAwaitingForSpawn.Remove(ballParameter);
                }

                yield return new WaitForSeconds(0.25f);
            }
        }

        private bool FindSpawnPosition(out Vector2 position, out float diameter) {
            diameter = 0;
            position = default;

            if (spawnedBalls.Count >= maxSpawnedBallsAtOnce)
                return false;

            int maxTries = 50;

            for (int i = 0; i < maxTries; i++) {
                position = new Vector2(Random.Range(spawnArea.min.x, spawnArea.max.x), Random.Range(spawnArea.min.y, spawnArea.max.y));
                diameter = Random.Range(1f, 1.2f);

                // check if not colliding with border
                if ((Mathf.Abs(position.x - spawnArea.min.x) > diameter && Mathf.Abs(position.y - spawnArea.min.y) > 1f
                    && Mathf.Abs(position.x - spawnArea.max.x) > diameter && Mathf.Abs(position.y - spawnArea.max.y) > 1f) == false)
                    continue;

                // check if not colliding with other balls
                bool posOk = true;

                foreach (var ball in spawnedBalls) {
                    if (Vector2.Distance(position, ball.transform.position) <= (diameter + ball.transform.localScale.x) / 2f + 0.25f) {
                        posOk = false;
                        break;
                    }
                }

                foreach (var ball in ballsAwaitingForSpawn) {
                    if (Vector2.Distance(position, ball.SpawnPosition) <= (diameter + ball.Diameter) / 2f + 0.25f) {
                        posOk = false;
                        break;
                    }
                }

                if (posOk) {
                    return true;
                }
            }

            return false;
        }
    }
}
