using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// NextScene: simple component that loads a scene when a Quest or Cutscene finishes.
// Usage:
// - Add this component to any GameObject in the scene (preferably under Manager).
// - Choose Trigger Type: Quest or Cutscene.
//   - If Quest: assign a `QuestInfoSO` in `questRef` or set `questId` string.
//   - If Cutscene: assign the cutscene GameObject in `cutsceneObject` (the component watches for it to be deactivated).
// - Fill `sceneName` with the exact Scene name as in Build Settings.
// When the chosen trigger completes, the component will call `SceneManager.LoadScene(sceneName)`.
public class NextScene : MonoBehaviour
{
    public enum TriggerType { Quest, Cutscene }
    public TriggerType triggerType = TriggerType.Quest;

    [Tooltip("Quest asset (preferred)")]
    public QuestInfoSO questRef;

    [Tooltip("Fallback quest id string (used if questRef is not assigned)")]
    public string questId;

    [Tooltip("Cutscene GameObject to watch. When it becomes inactive after being active, the scene will load.")]
    public GameObject cutsceneObject;

    [Tooltip("Scene name to load when trigger fires (must be in Build Settings)")]
    public string sceneName;

    [Header("Player Spawn")]
    [Tooltip("If true, the Player will be moved to `playerSpawnPosition` after the new scene loads.")]
    public bool setPlayerPositionOnLoad = true;

    [Tooltip("Player position to set after loading the target scene.")]
    public Vector3 playerSpawnPosition = new Vector3(-122.6f, -196.8f, 0f);

    [Tooltip("If true, the scene will be loaded additively using LoadSceneMode.Additive")]
    public bool loadAdditively = false;

    private bool triggered = false;

    private void OnEnable()
    {
        QuestEvents.OnQuestStateChanged += OnQuestStateChanged_Global;
        if (triggerType == TriggerType.Cutscene && cutsceneObject != null)
        {
            StartCoroutine(MonitorCutscene());
        }
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestStateChanged -= OnQuestStateChanged_Global;
        StopAllCoroutines();
    }

    private void Start()
    {
        // If trigger is quest and the quest is already finished at Start, trigger immediately
        if (triggerType == TriggerType.Quest && !triggered)
        {
            string id = ResolveQuestId();
            if (!string.IsNullOrEmpty(id) && QuestManager.Instance != null)
            {
                var state = QuestManager.Instance.GetQuestState(id);
                if (state == QuestState.FINISHED) TriggerLoad();
            }
        }
    }

    private string ResolveQuestId()
    {
        if (questRef != null && !string.IsNullOrEmpty(questRef.Id)) return questRef.Id;
        return questId;
    }

    private void OnQuestStateChanged_Global(string qid, QuestState newState)
    {
        if (triggered) return;
        if (triggerType != TriggerType.Quest) return;
        string id = ResolveQuestId();
        if (string.IsNullOrEmpty(id)) return;
        if (qid == id && newState == QuestState.FINISHED)
        {
            TriggerLoad();
        }
    }

    private IEnumerator MonitorCutscene()
    {
        // Wait until the cutscene is used (becomes active), then wait until it's deactivated.
        while (!triggered)
        {
            // wait until active
            while (cutsceneObject != null && !cutsceneObject.activeSelf)
            {
                yield return null;
            }

            // now wait until it's deactivated
            while (cutsceneObject != null && cutsceneObject.activeSelf)
            {
                yield return null;
            }

            // cutscene went from active -> inactive: trigger
            if (cutsceneObject != null && !triggered)
            {
                TriggerLoad();
                yield break;
            }

            yield return null;
        }
    }

    private void TriggerLoad()
    {
        if (triggered) return;
        triggered = true;

        // guard: don't attempt to load the same scene that's already active
        var active = SceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(sceneName) && active.name == sceneName)
        {
            Debug.Log($"NextScene: active scene already '{sceneName}', skipping load (GameObject={gameObject.name}).");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            // Fallback: try to load next scene by build index
            int nextIndex = active.buildIndex + 1;
            int max = SceneManager.sceneCountInBuildSettings;
            if (nextIndex >= 0 && nextIndex < max)
            {
                Debug.Log($"NextScene: sceneName empty on '{gameObject.name}', loading next build-index scene {nextIndex}.");
                SceneManager.LoadScene(nextIndex);
                return;
            }

            Debug.LogWarning($"NextScene: sceneName is empty on '{gameObject.name}' and no next scene in Build Settings.");
            return;
        }

        // If this is the main Menu, prefer replacing the scene (Single) to avoid stacking UI/managers
        bool useAdditive = loadAdditively;
        if (sceneName.Equals("Menu", System.StringComparison.OrdinalIgnoreCase))
        {
            useAdditive = false;
            Debug.Log($"NextScene: overriding additive load for Menu -> using Single (GameObject={gameObject.name}).");
        }

        Debug.Log($"NextScene: loading scene '{sceneName}' (additive={useAdditive}) (GameObject={gameObject.name})");

        // If requested, register a one-shot callback to set the player's position after the scene finishes loading.
        if (setPlayerPositionOnLoad)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded_SetPlayerPos;
        }

        if (useAdditive)
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        else
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void OnSceneLoaded_SetPlayerPos(Scene scene, LoadSceneMode mode)
    {
        // Only run once; unsubscribe immediately
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded_SetPlayerPos;

        if (!setPlayerPositionOnLoad) return;

        // If sceneName is set, only apply when the loaded scene matches (or if sceneName empty, apply to any next scene)
        if (!string.IsNullOrEmpty(sceneName) && !scene.name.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            return;

        // Prefer to configure PlayerSpawn if present; otherwise fall back to moving the Player directly.
        var playerSpawn = UnityEngine.Object.FindObjectOfType<PlayerSpawn>();
        if (playerSpawn != null)
        {
            playerSpawn.SetSpawnPosition(playerSpawnPosition);
            Debug.Log($"NextScene: set PlayerSpawn position to {playerSpawnPosition} for scene '{scene.name}'");
        }
        else
        {
            // Try to find Player and move directly as a fallback
            var player = UnityEngine.Object.FindFirstObjectByType<Player>();
            if (player != null)
            {
                player.transform.position = playerSpawnPosition;
                Debug.Log($"NextScene: moved Player to {playerSpawnPosition} after loading scene '{scene.name}'");
            }
            else
            {
                Debug.LogWarning($"NextScene: could not find Player or PlayerSpawn to set spawn after scene '{scene.name}' loaded.");
            }
        }
    }
}
