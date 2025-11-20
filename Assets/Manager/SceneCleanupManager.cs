using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneCleanupManager : MonoBehaviour
{
    // A small persistent helper that ensures there's only one active AudioListener and one EventSystem
    // after each scene load. Attach to a persistent manager GameObject (e.g., GameManager root).

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        // run once for current scene
        DeduplicateAudioAndEventSystems();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DeduplicateAudioAndEventSystems();
    }

    private void DeduplicateAudioAndEventSystems()
    {
        // AudioListener: keep first enabled, disable others
        var listeners = FindObjectsOfType<AudioListener>();
        bool keep = true;
        foreach (var l in listeners)
        {
            if (l == null) continue;
            if (keep && l.enabled)
            {
                keep = false;
                continue;
            }
            // disable duplicates
            l.enabled = false;
        }

        // EventSystem: keep first active, destroy or disable others
        var systems = FindObjectsOfType<EventSystem>();
        bool kept = false;
        foreach (var es in systems)
        {
            if (es == null) continue;
            if (!kept && es.gameObject.activeInHierarchy)
            {
                kept = true;
                continue;
            }
            // Prefer disabling to destroying to avoid removing designer's prefab instances.
            es.gameObject.SetActive(false);
        }
    }
}
