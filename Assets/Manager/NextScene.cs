using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// NextScene: Automatically loads the next scene when ANY quest finishes.
/// Configure quest → scene mappings in the inspector.
/// </summary>
public class NextScene : MonoBehaviour
{
    [System.Serializable]
    public class QuestSceneMapping
    {
        [Tooltip("Quest that triggers scene transition")]
        public QuestInfoSO questRef;
        [Tooltip("Fallback quest ID")]
        public string questId;
        [Tooltip("Scene to load")]
        public string sceneName;
        [Tooltip("Player spawn position")]
        public Vector3 playerSpawnPosition = Vector3.zero;
        [Tooltip("Load additively")]
        public bool loadAdditively = false;
    }

    [Header("Quest → Scene Mappings")]
    [Tooltip("When any of these quests finish, load the corresponding scene")]
    public QuestSceneMapping[] questMappings = new QuestSceneMapping[0];

    [Header("Delay Before Loading")]
    [Tooltip("Delay (in seconds) before loading the next scene")]
    public float delayBeforeLoad = 2f;

    [Header("Player Spawn")]
    [Tooltip("Set player position after scene loads")]
    public bool setPlayerPositionOnLoad = true;

    private Dictionary<string, QuestSceneMapping> mappingCache = new Dictionary<string, QuestSceneMapping>();
    private HashSet<string> triggeredQuests = new HashSet<string>(); // Track which quests already triggered

    private void OnEnable()
    {
        QuestEvents.OnQuestStateChanged += OnQuestFinished;
        BuildCache();
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestStateChanged -= OnQuestFinished;
        triggeredQuests.Clear();
    }

    private void BuildCache()
    {
        mappingCache.Clear();
        foreach (var mapping in questMappings)
        {
            if (mapping == null) continue;
            string id = mapping.questRef != null ? mapping.questRef.Id : mapping.questId;
            if (!string.IsNullOrEmpty(id))
            {
                mappingCache[id] = mapping;
                Debug.Log($"  - Quest '{id}' → Scene '{mapping.sceneName}'");
            }
        }
        Debug.Log($"NextScene: Loaded {mappingCache.Count} quest mappings");
    }

    private void OnQuestFinished(string questId, QuestState state)
    {
        Debug.Log($"NextScene.OnQuestFinished: questId={questId}, state={state}");
        
        if (triggeredQuests.Contains(questId))
        {
            Debug.Log($"  -> Quest '{questId}' already triggered, ignoring");
            return;
        }
        
        if (state != QuestState.FINISHED)
        {
            Debug.Log($"  -> State is not FINISHED, ignoring");
            return;
        }

        if (mappingCache.TryGetValue(questId, out var mapping))
        {
            triggeredQuests.Add(questId);
            Debug.Log($"NextScene: Quest '{questId}' finished → Loading '{mapping.sceneName}'");
            
            // Signal TurnOff to fade light to dark
            var turnOff = FindFirstObjectByType<TurnOff>();
            if (turnOff != null)
            {
                turnOff.TurnOffLight();
                Debug.Log("NextScene: Signaled TurnOff to fade light");
            }
            
            LoadScene(mapping);
        }
        else
        {
            Debug.Log($"  -> Quest '{questId}' not found in mappings. Available: {string.Join(", ", mappingCache.Keys)}");
        }
    }

    private void LoadScene(QuestSceneMapping mapping)
    {
        if (string.IsNullOrEmpty(mapping.sceneName))
        {
            Debug.LogWarning("NextScene: Scene name is empty");
            return;
        }

        var current = SceneManager.GetActiveScene();
        if (current.name == mapping.sceneName)
        {
            Debug.Log($"NextScene: Already on '{mapping.sceneName}'");
            return;
        }

        Debug.Log($"NextScene: Waiting {delayBeforeLoad} seconds before loading '{mapping.sceneName}'");
        StartCoroutine(LoadSceneWithDelay(mapping));
    }

    private IEnumerator LoadSceneWithDelay(QuestSceneMapping mapping)
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        Debug.Log($"NextScene: Now loading '{mapping.sceneName}'");

        if (setPlayerPositionOnLoad)
        {
            var pos = mapping.playerSpawnPosition;
            SceneManager.sceneLoaded += (scene, mode) => SetPlayerPosition(pos);
        }

        var loadMode = mapping.loadAdditively ? LoadSceneMode.Additive : LoadSceneMode.Single;
        SceneManager.LoadScene(mapping.sceneName, loadMode);
    }

    private void SetPlayerPosition(Vector3 position)
    {
        SceneManager.sceneLoaded -= (s, m) => SetPlayerPosition(position);

        var spawn = FindFirstObjectByType<PlayerSpawn>();
        if (spawn != null)
        {
            spawn.SetSpawnPosition(position);
            Debug.Log($"NextScene: Set spawn position to {position}");
            return;
        }

        var player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.transform.position = position;
            Debug.Log($"NextScene: Moved player to {position}");
        }
    }
}
