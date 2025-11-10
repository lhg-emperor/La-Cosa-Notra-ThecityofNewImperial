using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;


    private GameData gameData;

    private List<IDataPersitence> dataPersistenceObjects;

    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one DataPersistenceManager in the Scene");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // Đường dẫn cố định theo yêu cầu
        string desiredRoot = @"D:\DataLacosanotra\SaveData";

        string saveDirectory = desiredRoot;

        // Đặt tên file mặc định nếu inspector để trống
        string effectiveFileName = string.IsNullOrWhiteSpace(fileName) ? "gameData.json" : fileName;

        try
        {
            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(saveDirectory);
        }
        catch (Exception e)
        {
            Debug.LogError($"Không thể tạo thư mục lưu tại '{saveDirectory}'. Lưu về Application.persistentDataPath. Exception: {e}");
            saveDirectory = Application.persistentDataPath;
        }

        Debug.Log("Save folder: " + saveDirectory);

        this.dataHandler = new FileDataHandler(saveDirectory, effectiveFileName);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing to defaults.");
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
        foreach (IDataPersitence dataPersitenceObj in dataPersistenceObjects)
        {
            dataPersitenceObj.SaveData(ref gameData);
        }
        Debug.Log("Save Time Count = " + gameData.timeCount);
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersitence> FindAllDataPersistenceObjects()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersitence>()
            .ToList();
    }
}
