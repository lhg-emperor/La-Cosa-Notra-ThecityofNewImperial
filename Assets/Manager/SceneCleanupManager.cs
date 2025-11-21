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
        DontDestroyOnLoad(transform.root.gameObject);
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

        // EventSystem: keep first active, destroy others to avoid duplicate ES errors
        var systems = UnityEngine.Object.FindObjectsByType<EventSystem>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
        EventSystem keepOne = null;
        foreach (var es in systems)
        {
            if (es == null) continue;
            if (keepOne == null && es.gameObject.activeInHierarchy)
            {
                keepOne = es;
                continue;
            }
            if (es != keepOne)
            {
                // Destroy duplicate EventSystem GameObjects
                Destroy(es.gameObject);
            }
        }
    }
}
