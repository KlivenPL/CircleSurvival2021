using System;
using UnityEngine;

namespace Assets.Source.Balls {
    [Serializable]
    class BallSpawnerStage {
        [SerializeField] private float endTime;
        [SerializeField] [Range(0f, 5f)] private float minTimeBetweenSpawns;
        [SerializeField] [Range(0f, 5f)] private float maxTimeBetweenSpawns;
        [SerializeField] [Range(1, 6)] private int ballPoolMinSize;
        [SerializeField] [Range(1, 6)] private int ballPoolMaxSize;
        [SerializeField] [Range(0, 6)] private int bombPoolMaxSize;
        [SerializeField] [Range(0f, 5f)] private float defuseMinTime;
        [SerializeField] [Range(0f, 5f)] private float defuseMaxTime;

        public float EndTime { get => endTime; set => endTime = value; }
        public int BallPoolMinSize { get => ballPoolMinSize; set => ballPoolMinSize = value; }
        public int BallPoolMaxSize { get => ballPoolMaxSize; set => ballPoolMaxSize = value; }
        public int BombPoolMaxSize { get => bombPoolMaxSize; set => bombPoolMaxSize = value; }
        public float DefuseMinTime { get => defuseMinTime; set => defuseMinTime = value; }
        public float DefuseMaxTime { get => defuseMaxTime; set => defuseMaxTime = value; }
        public float MinTimeBetweenSpawns { get => minTimeBetweenSpawns; set => minTimeBetweenSpawns = value; }
        public float MaxTimeBetweenSpawns { get => maxTimeBetweenSpawns; set => maxTimeBetweenSpawns = value; }
    }
}
