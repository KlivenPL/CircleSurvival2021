using UnityEngine;
using Zenject;

namespace Assets.Source.Cameras {
    class CameraManager : MonoBehaviour {
        new private Camera camera;

        [Inject]
        private void Init(
            [Inject(Id = DI.InjectId.GameCamera)] Camera camera) {
            this.camera = camera;
        }

        public Bounds CalculateCameraBounds() {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }
    }
}
