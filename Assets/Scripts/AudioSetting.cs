using UnityEngine;
using UnityEngine.Audio;

public class AudioSetting : MonoBehaviour
{
    public AudioMixer mixer;
    public SettingData data; // gán ScriptableObject vào đây

    public void ApplyAudio()
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(data.masterVolume) * 20);
        mixer.SetFloat("MusicVolume", Mathf.Log10(data.musicVolume) * 20);
        mixer.SetFloat("SFXVolume", Mathf.Log10(data.sfxVolume) * 20);
    }

    public void SaveAudio()
    {
        PlayerPrefs.SetFloat("MasterVolume", data.masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", data.musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", data.sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadAudio()
    {
        data.masterVolume = PlayerPrefs.GetFloat("MasterVolume", data.masterVolume);
        data.musicVolume = PlayerPrefs.GetFloat("MusicVolume", data.musicVolume);
        data.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", data.sfxVolume);
        ApplyAudio();
    }
}
