using System;
using UnityEngine;

namespace Assets.Source.Balls {
    partial class BallInitialParameters {

        public float DefuseTime { get; private set; }
        public float Diameter { get; private set; }
        public Vector2 SpawnPosition { get; private set; }

        public Action<BallFacade> SpawnAction { get; private set; }

        private BallInitialParameters() {

        }
    }
}
