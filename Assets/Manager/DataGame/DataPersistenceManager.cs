using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;


    private GameData gameData;
    public GameData CurrentGameData => gameData;


    private List<IDataPersitence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    private string selectedProfileId = "Em Là Của Anh Đừng Của Ai";
    public static DataPersistenceManager instance { get; private set; }
    // When set to true before calling LoadGame(), indicates the load was requested
    // explicitly by the player (Load Game button). Used to decide whether
    // Player should be positioned from saved data or from scene spawn points.
    public bool RequestedLoad { get; set; } = false;

    // True if the last LoadGame() call originated from a requested load
    public bool LastLoadFromMenu { get; set; } = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one DataPersistenceManager in the Scene");
            // Destroy the root GameObject to match DontDestroyOnLoad usage and avoid orphaned children
            Destroy(transform.root.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
        // Khởi tạo Data Handler
        string desiredRoot = @"D:\DataLacosanotra\SaveData";

        string saveDirectory = desiredRoot;
        string effectiveFileName = string.IsNullOrWhiteSpace(fileName) ? "gameData.json" : fileName;

        try
        {

            Directory.CreateDirectory(saveDirectory);
        }
        catch (Exception e)
        {
            Debug.LogError($"Không thể tạo thư mục lưu tại '{saveDirectory}'. Lưu về Application.persistentDataPath. Exception: {e}");
            saveDirectory = Application.persistentDataPath;
        }

        Debug.Log("Save folder: " + saveDirectory);

        this.dataHandler = new FileDataHandler(saveDirectory, effectiveFileName);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        // Defer loading one frame so scene objects (QuestManager, QuestPoints, EventSystem) finish enabling.
        StartCoroutine(WaitForSceneAndLoad());
    }

    private System.Collections.IEnumerator WaitForSceneAndLoad()
    {
        // wait one frame to allow other sceneLoaded handlers to run and Awake/OnEnable to complete
        yield return null;

        // small additional wait to allow slower initialization; loop briefly until QuestManager ready or timeout
        var qm = UnityEngine.Object.FindFirstObjectByType<QuestManager>();
        float timeout = 1.0f;
        float t = 0f;
        while (qm != null && !qm.IsInitialized && t < timeout)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // Rebuild dataPersistenceObjects in case new objects were created during wait
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnLoaded(Scene scene)
    {
        Debug.Log("OnSceneUnLoaded: " + scene.name);
        SaveGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
        // Set default scene and player position based on current active scene
        string active = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        gameData.currentSceneName = active;
        // Default player spawn for New Game
        gameData.playerPosition = new UnityEngine.Vector3(-110.17f, -129.1f, 0f);

        // If starting in FalconeHome scene, use its specific default spawn override
        if (!string.IsNullOrEmpty(active) && active.IndexOf("FalconeHome", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            gameData.playerPosition = new UnityEngine.Vector3(-122.5f, -199.4f, 0f);
        }
        // Persist the new game immediately so subsequent Load() won't load an old save
        try
        {
            dataHandler.Save(gameData, selectedProfileId);
            Debug.Log("NewGame: initial game data saved for profile: " + selectedProfileId);
        }
        catch (Exception e)
        {
            Debug.LogWarning("NewGame: failed to save initial game data: " + e.Message);
        }
    }

    public void LoadGame()
    {
        // Record whether this LoadGame invocation was explicitly requested
        LastLoadFromMenu = RequestedLoad;
        // Reset the request flag so subsequent automatic loads don't count as menu loads
        RequestedLoad = false;

        this.gameData = dataHandler.Load(selectedProfileId);

        if (this.gameData == null && initializeDataIfNull)
        {
            Debug.Log("No data was found. Initializing to defaults.");
            NewGame();
        }

        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing to defaults. Destroying the newest one");
            NewGame();
        }

        foreach (IDataPersitence dataPersitenceObj in dataPersistenceObjects)
        {
            dataPersitenceObj.LoadData(gameData);
        }
        Debug.Log("Loaded Time Count = " + gameData.timeCount);
    }

    public void SaveGame()
    {
        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            dataPersistenceObjects = FindAllDataPersistenceObjects();
        }

        foreach (IDataPersitence dataPersitenceObj in dataPersistenceObjects)
        {
            if (dataPersitenceObj != null) // ← Kiểm tra null trước khi save
                dataPersitenceObj.SaveData(ref gameData);
        }

        gameData.currentSceneName = SceneManager.GetActiveScene().name;
        dataHandler.Save(gameData, selectedProfileId);

        // Lưu scene hiện tại
        gameData.currentSceneName = SceneManager.GetActiveScene().name;

    Debug.Log("Save Time Count = " + gameData.timeCount);
    dataHandler.Save(gameData, selectedProfileId);
    }

    private void OnApplicationQuit()
    {
        
    }

    private List<IDataPersitence> FindAllDataPersistenceObjects()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersitence>()
            .ToList();
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
}
