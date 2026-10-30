using UnityEngine;
using UnityEngine.Audio;
using System;
using AYellowpaper.SerializedCollections;

[Serializable]
public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup audioMixerGroup;

    [Header("BGM Settings")]
    public AudioSource bgmSource;
    public AudioClip defaultBGM;
    public string bgmMixerParam = "BGM_VOLUME";

    [Header("SFX Settings")]
    public AudioSource sfxSource;
    public string sfxMixerParam = "SFX_VOLUME";

    [SerializedDictionary("SFX Name", "Audio Clip")]
    public SerializedDictionary<string, AudioClip> sfxList = new SerializedDictionary<string, AudioClip>();

    [HideInInspector] public float savedVolume;
    [HideInInspector] public float savedBgmVolume;
    [HideInInspector] public float savedSfxVolume;

    public void Initialize()
    {
        savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        savedBgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1f);
        savedSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);

        SetAudioMixerValue();

        PlayBGM();
    }

    public void PlayBGM()
    {
        if (!defaultBGM) return;

        bgmSource.clip = defaultBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxList.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{sfxName}' tidak ditemukan!");
        }
    }

    public void AssignAudioMixer(AudioSource[] allAudioSource)
    {
        foreach (AudioSource audioSource in allAudioSource)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
    }

    private void SetAudioMixerValue()
    {
        audioMixer.SetFloat(bgmMixerParam, Mathf.Log10(Mathf.Clamp(savedBgmVolume, 0.0001f, 1f)) * 20f);
        audioMixer.SetFloat(sfxMixerParam, Mathf.Log10(Mathf.Clamp(savedSfxVolume, 0.0001f, 1f)) * 20f);
    }

    public void SetBgmVolume(float volume)
    {
        savedBgmVolume = Mathf.Clamp01(volume);
        audioMixer.SetFloat(bgmMixerParam, Mathf.Log10(Mathf.Clamp(savedBgmVolume, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat("BgmVolume", savedBgmVolume);
    }

    public void SetSfxVolume(float volume)
    {
        savedSfxVolume = Mathf.Clamp01(volume);
        audioMixer.SetFloat(sfxMixerParam, Mathf.Log10(Mathf.Clamp(savedSfxVolume, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat("SfxVolume", savedSfxVolume);
    }

    public AudioClip GetAudioByName(AudioClip[] audioArray, string name)
    {
        foreach (var clip in audioArray)
        {
            if (clip != null && clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }
}