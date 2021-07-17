using Assets.KLib.Source.Events;
using Assets.Source._Infrastructure.Pooling;
using Assets.Source._Infrastructure.Spawnable;
using Assets.Source.Cameras;
using Assets.Source.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Source.Balls {
    class BallSpawner : MonoBehaviour, IEventListener<GameOverEvent> {

        [SerializeField] private int maxSpawnedBallsAtOnce = 6;
        [SerializeField] private List<BallSpawnerStage> stages;

        private IEnumerator<BallSpawnerStage> currentStageEnumerator;
        private BallSpawnerStage currentStage;
        private List<BallFacade> spawnedBalls;
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
            spawnTimer = 0.5f;
        }

        public void Despawn(BallFacade ballFacade) {
            spawnedBalls.Remove(ballFacade);
            ballFacade.gameObject.Despawn();
        }

        public void OnEvent(GameOverEvent @event) {
            if (!gameOver) {
                StopAllCoroutines();
                gameOver = true;
            }
        }

        private void Update() {
            if (gameOver)
                return;

            if ((spawnTimer -= Time.deltaTime) < 0) {
                Spawn();
                spawnTimer = Random.Range(currentStageEnumerator.Current.MinTimeBetweenSpawns, currentStageEnumerator.Current.MaxTimeBetweenSpawns);
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
            var bombsToSpawnCount = Random.Range(0, currentStageEnumerator.Current.BombPoolMaxSize + 1);
            var normalBallsToSpawnCount = Random.Range(currentStageEnumerator.Current.BallPoolMinSize, currentStageEnumerator.Current.BallPoolMaxSize + 1);

            var bombsToSpawn = CreateBalls(bombsToSpawnCount);
            var normalBallsToSpawn = CreateBalls(normalBallsToSpawnCount, bombsToSpawn.ToList());

            StartCoroutine(SpawnBalls(spawnBombs: true, bombsToSpawn));
            StartCoroutine(SpawnBalls(spawnBombs: false, normalBallsToSpawn));
        }

        private IEnumerable<BallInitialParameters> CreateBalls(int count, List<BallInitialParameters> awaitingBalls = null) {
            awaitingBalls ??= new List<BallInitialParameters>();

            for (int i = 0; i < count; i++) {
                if (FindSpawnPosition(awaitingBalls, out var position, out var diameter)) {
                    var ball = BallBuilder
                        .SetDiffuseTime(currentStageEnumerator.Current.DefuseMinTime, currentStageEnumerator.Current.DefuseMaxTime)
                        .SetSpawnPosition(position)
                        .SetDiameter(diameter)
                        .Build();

                    awaitingBalls.Add(ball);
                    yield return ball;
                }
            }
        }

        IEnumerator SpawnBalls(bool spawnBombs, IEnumerable<BallInitialParameters> ballInitialParameters) {
            if (spawnBombs)
                yield return new WaitForSeconds(Random.Range(0f, 0.75f));

            var spawnableType = spawnBombs ? SpawnableType.Ball_Bomb : SpawnableType.Ball_Normal;

            foreach (var ballParameter in ballInitialParameters) {
                poolManager.Spawn(spawnableType, (Vector3)ballParameter.SpawnPosition + new Vector3(0, 0, 5), Vector3.zero, ballParameter.Diameter * Vector3.one, out var spawnedBallGo);
                var spawnedBall = spawnedBallGo.GetComponent<BallFacade>();
                spawnedBall.OnSpawn(ballParameter);
                spawnedBalls.Add(spawnedBall);

                yield return new WaitForSeconds(0.25f);
            }
        }

        private bool FindSpawnPosition(IEnumerable<BallInitialParameters> alreadyAcceptedBalls, out Vector2 position, out float diameter) {
            diameter = 0;
            position = default;

            if (spawnedBalls.Count >= maxSpawnedBallsAtOnce)
                return false;

            int maxTries = 50;
            while (true) {
                position = new Vector2(Random.Range(spawnArea.min.x, spawnArea.max.x), Random.Range(spawnArea.min.y, spawnArea.max.y));
                diameter = Random.Range(1f, 1.2f);

                // check if not colliding with border
                if ((Mathf.Abs(position.x - spawnArea.min.x) > diameter && Mathf.Abs(position.y - spawnArea.min.y) > 1f
                    && Mathf.Abs(position.x - spawnArea.max.x) > diameter && Mathf.Abs(position.y - spawnArea.max.y) > 1f) == false)
                    continue;

                // check if not colliding with other balls
                bool posOk = true;
                for (int i = 0; i < spawnedBalls.Count; i++) {
                    if (Vector2.Distance(position, spawnedBalls[i].transform.position) <= diameter / 2f + spawnedBalls[i].Parameters.Diameter / 2f + 0.25f) {
                        posOk = false;
                        break;
                    }
                }

                foreach (var ball in alreadyAcceptedBalls) {
                    if (Vector2.Distance(position, ball.SpawnPosition) <= diameter / 2f + ball.Diameter / 2f + 0.25f) {
                        posOk = false;
                        break;
                    }
                }

                if (posOk) {
                    return true;
                }

                if (maxTries-- == 0) {
                    return false;
                }
            }
        }
    }
}
