using Assets.Source._Infrastructure.Pooling;
using UnityEngine;

public class Autodespawn : MonoBehaviour {

    [SerializeField] private float timer;
    private float defaultTimer;

    private void Awake() {
        defaultTimer = timer;
    }

    void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0 && timer != -1) {
            timer = -1;
            gameObject.Despawn();
        }
    }

    private void OnEnable() {
        timer = defaultTimer;
    }
}
