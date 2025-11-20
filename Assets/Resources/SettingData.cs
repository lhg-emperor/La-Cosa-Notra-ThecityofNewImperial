using UnityEngine;

[CreateAssetMenu(fileName = "SettingData", menuName = "Game Settings/Setting Data")]
public class SettingData : ScriptableObject
{
    [Range(0.0001f, 1f)] public float masterVolume = 1f;
    [Range(0.0001f, 1f)] public float musicVolume = 1f;
    [Range(0.0001f, 1f)] public float sfxVolume = 1f;
}
