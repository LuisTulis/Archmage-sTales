using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] soundClips;
    public AudioClip[] musicClips;
    public AudioClip[] ambienceClips;

    public AudioMixer audioMixer;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource ambienceSource;

    public static AudioManager Instance;

    private bool isDayAmbiencePlaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = FindSoundClip(soundName);
        if (clip != null)
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"Sound clip {soundName} not found!");
    }

    public void PlayMusic(string clipName, bool loop = true)
    {
        AudioClip clip = FindMusicClip(clipName);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
            Debug.LogWarning($"Music clip {clipName} not found!");
    }

    public void PlayAmbience(string clipName, bool loop = true)
    {
        AudioClip clip = FindAmbienceClip(clipName);

        if (clip != null)
        {
            ambienceSource.clip = clip;
            ambienceSource.loop = loop;
            ambienceSource.Play();
        }
        else
            Debug.LogWarning($"Ambience clip {clipName} not found!");
    }

    public AudioClip FindSoundClip(string soundName)
    {
        foreach (AudioClip clip in soundClips)
        {
            if (clip.name.Equals(soundName)) return clip;
        }
        return null;
    }

    public AudioClip FindMusicClip(string soundName)
    {
        foreach (AudioClip clip in musicClips)
        {
            if (clip.name.Equals(soundName)) return clip;
        }
        return null;
    }

    public AudioClip FindAmbienceClip(string soundName)
    {
        foreach (AudioClip clip in ambienceClips)
        {
            if (clip.name.Equals(soundName)) return clip;
        }
        return null;
    }

    public void StopMusic()
    {
        this.musicSource.Stop();
    }

    public void SetSoundVolume(float value)
    {
        audioMixer.SetFloat("SoundVolumeParam", value);
        PlayerPrefs.SetFloat("SoundVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolumeParam", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetAmbienceVolume(float value)
    {
        audioMixer.SetFloat("AmbienceVolumeParam", value);
        PlayerPrefs.SetFloat("AmbienceVolume", value);
    }

    public void MuteAll(bool mute)
    {
        audioMixer.SetFloat("MasterVolumeParam", mute ? -80f : 0f);
        PlayerPrefs.SetInt("Muted", mute ? 1 : 0);
    }

    public bool IsMuted()
    {
        return PlayerPrefs.GetInt("Muted", 0) == 1;
    }

    public void LoadVolumeSettings()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfx = PlayerPrefs.GetFloat("SoundVolume", 0.8f);
        float ambience = PlayerPrefs.GetFloat("AmbienceVolume", 0.8f);
        bool muted = PlayerPrefs.GetInt("Muted", 0) == 1;

        SetSoundVolume(sfx);
        SetMusicVolume(music);
        SetAmbienceVolume(ambience);
        MuteAll(muted);
    }

    public void HandleAmbience(float hour)
    {
        bool shouldBeDay = hour >= 6 && hour < 18;

        if (shouldBeDay == isDayAmbiencePlaying)
            return;

        isDayAmbiencePlaying = shouldBeDay;

        StartCoroutine(ChangeAmbience(
            shouldBeDay ? "day_ambience" : "night_ambience"
        ));
    }

    private IEnumerator ChangeAmbience(string clip)
    {
        AudioClip audioClip = this.FindAmbienceClip(clip);
        if (audioClip == null) yield break;

        // Fade OUT
        while (ambienceSource.volume > 0f)
        {
            ambienceSource.volume -= Time.deltaTime * 1f;
            yield return null;
        }

        ambienceSource.clip = audioClip;
        ambienceSource.loop = true;
        ambienceSource.Play();

        // Fade IN
        while (ambienceSource.volume < 1f)
        {
            ambienceSource.volume += Time.deltaTime * 1f;
            yield return null;
        }
    }

}
