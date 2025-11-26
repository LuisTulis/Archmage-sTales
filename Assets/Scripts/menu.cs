using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject options;
    public GameObject howToPlay;
    public GameObject credits;
    public GameObject exit;

    public AudioMixer mixer;

    public Slider soundSlider;
    public Slider musicSlider;
    public Slider ambienceSlider;
    public Toggle muteAllToggle;

    public void Start()
    {
        AudioManager.Instance.PlayMusic("WhisperingWillows", true);

        soundSlider.onValueChanged.AddListener(AudioManager.Instance.SetSoundVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        ambienceSlider.onValueChanged.AddListener(AudioManager.Instance.SetAmbienceVolume);
        muteAllToggle.onValueChanged.AddListener(AudioManager.Instance.MuteAll);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.8f);
        ambienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume", 0.8f);
        muteAllToggle.isOn = AudioManager.Instance.IsMuted();
    }

    public void OnOptions()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        howToPlay.SetActive(false);
        credits.SetActive(false);
        AudioManager.Instance.PlaySound("Click");
    }

    public void OnHowToPlay()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        howToPlay.SetActive(true);
        credits.SetActive(false);
        AudioManager.Instance.PlaySound("Click");
    }

    public void OnCredits()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        howToPlay.SetActive(false);
        credits.SetActive(true);
        AudioManager.Instance.PlaySound("Click");
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        howToPlay.SetActive(false);
        credits.SetActive(false);
        AudioManager.Instance.PlaySound("Click");
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnNewGame()
    {
        AudioManager.Instance.PlaySound("Click");
        SceneManager.LoadScene("Game_Scene_01");

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic("TheWanderer");
    }

    public void OnContinue()
    {
        AudioManager.Instance.PlaySound("Click");
        SceneManager.LoadScene("esteeselprototipo");

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic("TheWanderer");
    }
}
