using UnityEngine;

[DisallowMultipleComponent]
public class KeepSingleAudioListener : MonoBehaviour
{
    // Disable duplicate AudioListener components at Awake so additive loads don't cause conflicts.
    void Awake()
    {
        var listeners = FindObjectsOfType<AudioListener>();
        bool keepFirst = true;
        foreach (var l in listeners)
        {
            if (keepFirst) { keepFirst = false; continue; }
            if (l != null)
                l.enabled = false;
        }
    }
}
