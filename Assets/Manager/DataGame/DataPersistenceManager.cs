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

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one DataPersistenceManager in the Scene");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
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
    }

    public void LoadGame()
    {
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
