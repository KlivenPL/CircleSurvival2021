using System.Collections;
using UnityEngine;

namespace Assets.Source.Effects {
    public class Background : MonoBehaviour {
        [SerializeField] private int lineCount = 5;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private Color[] colorSchemes;
        [SerializeField] new private Camera camera;

        private LineRenderer[] lines;
        private int mainColorId;
        private Color _currColor;
        private Vector3 defaultAccelometerPos;

        public Color CurrentColor { get => _currColor; private set => _currColor = value; }

        public void Init() {
            StopAllCoroutines();
            defaultAccelometerPos = Input.acceleration;
            lines = new LineRenderer[lineCount * 4];

            for (int i = 0; i < lineCount * 2; i++) {
                for (int j = 0; j < 2; j++) {
                    var line = new GameObject("Background Line").AddComponent<LineRenderer>();
                    line.transform.SetParent(transform, true);
                    float z = j % 2 == 0 ? 2f : 6f;
                    float length = 10;
                    Vector3 posA = (i % 2 == 0 ? new Vector3(-length, -length, z) : new Vector3(-length, length, z)) + new Vector3(0, -lineCount / 2f + i / 2f);
                    Vector3 posB = (i % 2 == 0 ? new Vector3(length, length, z) : new Vector3(length, -length, z)) + new Vector3(0, -lineCount / 2f + i / 2f);
                    var positions = new Vector3[] { posA, posB };
                    line.material = lineMaterial;
                    line.gameObject.layer = 8;
                    line.SetPositions(positions);
                    line.startWidth = 0.03f / z;
                    line.endWidth = 0.03f / z;
                    line.sortingOrder = 1;

                    if (j % 2 == 1) {
                        line.startColor = new Color(1, 1, 1, 0.5f);
                        line.endColor = new Color(1, 1, 1, 0.5f);
                    }

                    lines[i + j] = line;
                    StartCoroutine(ChangePosition(line, posA, posB, z));
                }
            }
            lineMaterial.color = colorSchemes[Random.Range(0, colorSchemes.Length)];
            mainColorId = Random.Range(0, colorSchemes.Length);

            StartCoroutine(ChangeColor());
            enabled = true;
        }

        private void FixedUpdate() {
            CurrentColor = Color.Lerp(lineMaterial.color, colorSchemes[mainColorId], Time.deltaTime / 3f);
            _currColor.a = 1f;
            lineMaterial.color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 0.4f);
            camera.backgroundColor = CurrentColor * .25f;
        }

        IEnumerator ChangePosition(LineRenderer line, Vector3 defPosA, Vector3 defPosB, float z) {
            float range = 1.25f / z;
            while (true) {
                Vector3 newPos = new Vector3((-Input.acceleration.x + defaultAccelometerPos.x) * range, (-Input.acceleration.y + defaultAccelometerPos.y) * range);
                line.SetPosition(0, Vector3.Lerp(line.GetPosition(0), defPosA + newPos, Time.deltaTime * 3f / Time.timeScale));
                line.SetPosition(1, Vector3.Lerp(line.GetPosition(1), defPosB + newPos, Time.deltaTime * 3f / Time.timeScale));
                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator ChangeColor() {
            yield return new WaitForSeconds(5f);
            mainColorId = Random.Range(0, colorSchemes.Length);
            StartCoroutine(ChangeColor());
        }
    }
}