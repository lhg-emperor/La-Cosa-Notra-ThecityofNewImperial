using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
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
        //
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
