using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer myMixer;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Check if we have saved volume settings. If so, load them!
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            // If this is the player's very first time opening the game, force the mixer to match the sliders
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        // The math to convert a 0.0001 - 1 slider into decibels!
        myMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
}