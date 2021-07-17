using UnityEngine;

namespace Assets.Source.Balls {
    partial class BallInitialParameters {
        public partial class Builder {

            private float defuseTime;
            private float diameter;
            private Vector2 spawnPosition;

            public BallInitialParameters Build() {
                return new BallInitialParameters {
                    DefuseTime = defuseTime,
                    Diameter = diameter,
                    SpawnPosition = spawnPosition
                };
            }

            public Builder SetDiffuseTime(float minDefuseTime, float maxDefuseTime) {
                defuseTime = Random.Range(minDefuseTime, maxDefuseTime);
                return this;
            }

            public Builder SetDiameter(float diameter) {
                this.diameter = diameter;
                return this;
            }

            public Builder SetSpawnPosition(Vector2 spawnPosition) {
                this.spawnPosition = spawnPosition;
                return this;
            }
        }
    }
}
