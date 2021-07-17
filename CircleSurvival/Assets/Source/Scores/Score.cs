using Assets.KLib.Source.Events;
using Assets.KLib.Source.Interfaces;
using Assets.Source.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Source.Scores {
    class Score : IFixedTickable, IEventListener<GameOverEvent> {

        private const string HighscoreSave = "Highscore";

        private readonly ISaveManager saveManager;
        private readonly Menu menu;
        private float _score;
        private bool isPlaying;

        public Score(ISaveManager saveManager, Menu menu) {
            this.saveManager = saveManager;
            this.menu = menu;

            this.ListenToEvent<GameOverEvent>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public float ScoreValue {
            get => _score;
            set {
                _score = value;
                menu.SetGameScoreText(value);
            }
        }

        public float HighscoreValue {
            get => saveManager.GetFloat(HighscoreSave);
            private set => saveManager.SetFloat(HighscoreSave, value);
        }

        public void FixedTick() {
            if (isPlaying) {
                ScoreValue += Time.fixedDeltaTime;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1) {
            if (scene.buildIndex != 0) {
                isPlaying = true;
                ScoreValue = 0;
            }
        }

        public void OnEvent(GameOverEvent @event) {
            isPlaying = false;
            if (ScoreValue > HighscoreValue) {
                HighscoreValue = ScoreValue;
            }
        }
    }
}
