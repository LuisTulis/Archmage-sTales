using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    public Slider soundSlider;
    public Slider musicSlider;
    public Slider ambienceSlider;
    public Toggle muteAllToggle;

    public Button resumeButton;
    public Button quitButton;

    public void Start()
    {
        pauseMenu.SetActive(false);

        soundSlider.onValueChanged.AddListener(AudioManager.Instance.SetSoundVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        ambienceSlider.onValueChanged.AddListener(AudioManager.Instance.SetAmbienceVolume);
        muteAllToggle.onValueChanged.AddListener(AudioManager.Instance.MuteAll);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.8f);
        ambienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume", 0.8f);
        muteAllToggle.isOn = AudioManager.Instance.IsMuted();

        resumeButton.onClick.AddListener(this.OnResume);
        quitButton.onClick.AddListener(this.OnQuit);
    }

    public void OnResume()
    {
        pauseMenu.SetActive(false);
        GameManager.Instance.isPaused = false;
    }

    public void OnQuit()
    {
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene("Menu_Scene_01");
    }

}
