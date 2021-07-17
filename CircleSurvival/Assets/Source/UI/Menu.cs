using Assets.KLib.Source.Events;
using Assets.KLib.Source.Interfaces;
using Assets.Source.Effects;
using Assets.Source.Events;
using Assets.Source.Scores;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

class Menu : MonoBehaviour, IEventListener<GameOverEvent> {
    private const string ThemedCirclesEnabledSave = "ThemedCirclesEnabled";

    [SerializeField] private Button playBtn;
    [SerializeField] private RectTransform mainMenuTr;
    [SerializeField] private RectTransform gameTr;
    [SerializeField] private RectTransform gameOverTr;

    [SerializeField] private Text gameScoreTxt;
    [SerializeField] private Text gameOverScoreTxt;
    [SerializeField] private Text gameOverHighscoreTxt;
    [SerializeField] private Text menuHighscoreTxt;
    [SerializeField] private Text newHighscoreTxt;

    [SerializeField] private Toggle themedCirclesChk;
    [SerializeField] private Toggle musicChk;
    [SerializeField] private Toggle soundsChk;

    private Background background;
    private AudioManager audioManager;
    private ISaveManager saveManager;
    private Score score;
    private bool isReplay;

    public Vector3 GameOverTrDefPos { get; set; }

    public bool ThemedCirclesEnabled {
        get => saveManager.GetInt(ThemedCirclesEnabledSave, 1) == 1;
        set => saveManager.SetInt(ThemedCirclesEnabledSave, value ? 1 : 0);
    }

    public void SetGameScoreText(float value) {
        gameScoreTxt.text = value.ToString("0");
    }

    [Inject]
    private void Init(AudioManager audioManager, Background background, Score score, ISaveManager saveManager) {
        this.audioManager = audioManager;
        this.saveManager = saveManager;
        this.background = background;
        this.score = score;

        this.ListenToEvent<GameOverEvent>();
    }

    void Start() {
        GameOverTrDefPos = gameOverTr.transform.position;
        gameOverTr.transform.position = new Vector3(0, -10000, 0);
        gameOverTr.gameObject.SetActive(true);
        menuHighscoreTxt.text = score.HighscoreValue.ToString("00.00").Replace(NumberDecimalSeparator, ":");
        themedCirclesChk.isOn = ThemedCirclesEnabled;
        musicChk.isOn = audioManager.MusicVolumeValue != -80;
        soundsChk.isOn = audioManager.SfxVolumeValue != -80;

        background.Init();
        StartCoroutine(Animate());

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void PlayBtn() {
        StopAllCoroutines();
        mainMenuTr.gameObject.SetActive(false);
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        gameTr.gameObject.SetActive(true);
    }

    public void ReplayBtn() {
        isReplay = true;
        SceneManager.UnloadSceneAsync(1);
    }

    public void MenuBtn() {
        isReplay = false;
        SceneManager.UnloadSceneAsync(1);
    }

    public void OnThemedCirclesChange() {
        ThemedCirclesEnabled = themedCirclesChk.isOn;
    }

    public void OnMusicChkChange() {
        audioManager.MusicVolumeValue = musicChk.isOn ? -6f : -80f;
    }

    public void OnSoundChkChange() {
        audioManager.SfxVolumeValue = soundsChk.isOn ? 0f : -80f;
    }

    void OnSceneUnloaded(Scene scene) {
        if (scene.buildIndex != 1)
            return;

        menuHighscoreTxt.text = score.HighscoreValue.ToString("00.00").Replace(NumberDecimalSeparator, ":");
        gameOverTr.transform.position = new Vector3(0, -10000, 0);

        if (isReplay) {
            isReplay = false;
            PlayBtn();
            return;
        }
        /* Camera.main.GetComponent<VignetteAndChromaticAberration>().blur = 1;
         Camera.main.GetComponent<VignetteAndChromaticAberration>().blurSpread = 1;*/
        StartCoroutine(Animate());
        mainMenuTr.gameObject.SetActive(true);
    }


    IEnumerator Animate() {
        while (true) {
            playBtn.transform.localScale = (1.2f + Mathf.Sin(Time.time * Time.timeScale) * .1f) * Vector3.one;
            playBtn.image.color = background.CurrentColor * 1.25f;
            if (themedCirclesChk.isOn)
                themedCirclesChk.graphic.color = background.CurrentColor * 1.25f;
            yield return new WaitForFixedUpdate();
        }
    }

    public void OnEvent(GameOverEvent @event) {
        StartCoroutine(ShowGameOverScreen());
    }

    private IEnumerator ShowGameOverScreen() {
        yield return new WaitForSeconds(1f);
        gameOverScoreTxt.text = score.ScoreValue.ToString("00.00").Replace(NumberDecimalSeparator, ":");
        gameOverHighscoreTxt.text = score.HighscoreValue.ToString("00.00").Replace(NumberDecimalSeparator, ":");

        newHighscoreTxt.gameObject.SetActive(score.ScoreValue > score.HighscoreValue);
        gameTr.gameObject.SetActive(false);
        gameOverTr.transform.position = GameOverTrDefPos;
    }

    private static string NumberDecimalSeparator {
        get {
            return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        }
    }
}
