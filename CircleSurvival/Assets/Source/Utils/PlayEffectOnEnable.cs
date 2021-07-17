using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PlayEffectOnEnable : MonoBehaviour {

    private ParticleSystem ps;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable() {
        ps.Play();
    }

    private void OnDisable() {
        ps.Stop();
    }
}
