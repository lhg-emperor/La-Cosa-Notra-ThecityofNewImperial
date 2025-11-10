using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Setting : MonoBehaviour
{
    public Slider MasterVol, MusicVol, SFXVol;
    public AudioMixer mainAudioMixer;

    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("Master Vol", MasterVol.value);
    }
    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("Music Vol", MusicVol.value);
    }
    public void ChangeSFXVolume()
    {
        mainAudioMixer.SetFloat("SFX Vol", SFXVol.value);
    }
}
