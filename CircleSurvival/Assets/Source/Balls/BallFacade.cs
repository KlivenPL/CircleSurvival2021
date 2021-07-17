using Assets.KLib.Source.Events;
using Assets.KLib.Source.Input;
using Assets.KLib.Source.Utils.Rotation;
using Assets.Source.Events;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Assets.Source.Balls {
    class BallFacade : MonoBehaviour, IEventListener<GameOverEvent> {

        private BallBehaviourBase ballBehaviour;
        private BallSpawner ballSpawner;
        private AudioManager audioManager;
        new private Rigidbody2D rigidbody;
        new private Camera camera;
        private IMobileInput mobileInput;

        public bool IsGameOver { get; private set; }
        public BallInitialParameters Parameters { get; private set; }

        [Inject]
        private void Init(
            BallBehaviourBase ballBehaviour,
            IMobileInput mobileInput,
            [Inject(Id = DI.InjectId.GameCamera)] Camera camera,
            BallSpawner ballSpawner,
            AudioManager audioManager,
            Rigidbody2D rigidbody) {

            this.ListenToEvent<GameOverEvent>();

            this.ballBehaviour = ballBehaviour;
            this.ballSpawner = ballSpawner;
            this.camera = camera;
            this.audioManager = audioManager;
            this.rigidbody = rigidbody;
            this.mobileInput = mobileInput;

            rigidbody.gravityScale = 0f;
        }

        public void OnSpawn(BallInitialParameters parameters) {
            Parameters = parameters;
            mobileInput.SingleTouchEvent += OnTouchInput;
            audioManager.PlaySpawnSfx();
            ballBehaviour.OnSpawn();
        }

        public void OnEvent(GameOverEvent @event) {
            IsGameOver = true;

            if (!this) {
                return;
            }

            StopAllCoroutines();
            rigidbody.isKinematic = false;

            if (!gameObject.activeSelf) {
                return;
            }

            if (@event.BallFacade == this) {
                StartCoroutine(transform.KScale(transform.localScale, transform.localScale * 3f, 0.5f));
            } else {
                if (@event.BallFacade.ballBehaviour is NormalBallBehaviour) {
                    AddExplosionForce(@event.BallFacade.transform.position);
                } else {
                    StartCoroutine(SuckInAnimation(@event.BallFacade));
                }
            }
        }

        private void OnTouchInput(object sender, Vector2 screenPos) {
            if (gameObject.activeSelf == false) {
                return;
            }

            var worldPos = camera.ScreenToWorldPoint(screenPos);
            if (Vector2.Distance(transform.position, worldPos) <= Parameters.Diameter / 2.0f) {
                ballBehaviour.OnTouch();
            }
        }

        public void Despawn() {
            ballSpawner.Despawn(this);
        }

        private void AddExplosionForce(Vector2 position) {
            rigidbody.AddForce(((Vector2)transform.position - position).normalized * 9.81f, ForceMode2D.Impulse);
        }

        private IEnumerator SuckInAnimation(BallFacade blackHole) {
            var originalDistance = Vector2.Distance(blackHole.transform.position, (Vector2)transform.position);
            var originalScale = transform.localScale;
            float distance;

            while ((distance = Vector2.Distance(blackHole.transform.position, (Vector2)transform.position)) > blackHole.Parameters.Diameter / 2f * 3f) {
                rigidbody.AddForce(((Vector2)blackHole.transform.position - (Vector2)transform.position).normalized * 10f, ForceMode2D.Force);
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, 1f - distance / originalDistance);
                yield return new WaitForFixedUpdate();
            }

            Despawn();
        }

        private void OnDisable() {
            Parameters = null;
            mobileInput.SingleTouchEvent -= OnTouchInput;
            StopAllCoroutines();
        }
    }
}
