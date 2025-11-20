using UnityEngine;

public interface IDataPersitence
{
    void LoadData(GameData data);

    void SaveData(ref GameData data);

}
