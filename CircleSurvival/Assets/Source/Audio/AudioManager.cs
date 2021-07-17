using Assets.KLib.Source.Events;
using Assets.KLib.Source.Interfaces;
using Assets.Source.Events;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Zenject;

public enum SoundEffect {
    Spawn1,
    Spawn2,
    Spawn3,
    Explosion1,
    Explosion2,
    Pop,
    BlackHole,
    BassDrop
}

class AudioManager : MonoBehaviour, IEventListener<GameOverEvent> {
    private const string MusicVolumeSave = "MusicVolume";
    private const string SfxVolumeSave = "SfxVolume";

    public AudioMixer audioMixer;

    public AudioClip[] music;
    public AudioClip[] soundEffects;

    public AudioSource musicAS;
    public AudioSource sfxAS;
    private ISaveManager saveManager;

    public float MusicVolumeValue {
        get => saveManager.GetFloat(MusicVolumeSave, -6f);
        set {
            audioMixer.SetFloat("musicVolume", value);
            saveManager.SetFloat(MusicVolumeSave, value);
        }
    }

    public float SfxVolumeValue {
        get => saveManager.GetFloat(SfxVolumeSave);
        set {
            audioMixer.SetFloat("sfxVolume", value);
            saveManager.SetFloat(SfxVolumeSave, value);
        }
    }

    [Inject]
    private void Init(ISaveManager saveManager) {
        this.saveManager = saveManager;
        MusicVolumeValue = MusicVolumeValue;
        SfxVolumeValue = SfxVolumeValue;
    }

    private void Awake() {
        this.ListenToEvent<GameOverEvent>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded; ;
    }

    private void OnSceneUnloaded(Scene scene) {
        if (scene.buildIndex != 0)
            PlayMenuMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1) {
        if (scene.buildIndex == 0)
            PlayMenuMusic();
        else
            PlayGameMusic();
    }

    public void PlayMenuMusic() {
        PlayMusic(music[0]);
    }

    public void PlayGameMusic() {
        AudioClip tmpMusic;
        do
            tmpMusic = music[Random.Range(1, music.Length)];
        while (tmpMusic == musicAS.clip);

        PlayMusic(tmpMusic);
    }

    public void PlayMusic(AudioClip music) {
        StopAllCoroutines();
        StartCoroutine(PlayMusicIE(music));
    }

    public void PlaySfx(SoundEffect sfx, float vol) {
        sfxAS.PlayOneShot(soundEffects[(int)sfx], vol);
    }

    public void PlaySpawnSfx() {
        PlaySfx((SoundEffect)Random.Range((int)SoundEffect.Spawn1, (int)SoundEffect.Spawn3 + 1), 0.5f);
    }

    public void PlayExplosionSfx() {
        PlaySfx((SoundEffect)Random.Range((int)SoundEffect.Explosion1, (int)SoundEffect.Explosion2 + 1), 0.5f);
    }

    public IEnumerator PlayMusicIE(AudioClip music) {
        var vol = 1f;
        if (musicAS.isPlaying) {
            while (musicAS.volume > 0) {
                musicAS.volume -= Time.deltaTime / Time.timeScale * 3f;
                yield return null;
            }
        }
        musicAS.volume = 0;
        musicAS.Stop();
        if (music != null) {
            musicAS.clip = music;
            musicAS.Play();
            while (musicAS.volume <= vol) {
                musicAS.volume += Time.deltaTime / Time.timeScale * 3f;
                yield return null;
            }
        }
    }

    public void OnEvent(GameOverEvent @event) {
        Debug.Log("GAME OVER!");
        musicAS.Stop();
    }
}
