using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int timeCount;
    public Vector3 playerPosition;

    // Thêm các thuộc tính của Player
    public int Health;
    public string CurrentWeapon;
    public bool IsInVehicle;

    // Thêm Scene hiện tại
    public string currentSceneName;

    // Player inventory and weapon info
    public int CurrentMagazineAmmo;
    public int TotalReserveAmmo;
    public List<string> WeaponSlots = new List<string>();
    public int ActiveWeaponIndex = -1;

    // Quests saved state
    [System.Serializable]
    public class QuestSaveEntry
    {
        public string id;
        public int state;
        public int currentStepIndex;
    }

    public List<QuestSaveEntry> quests = new List<QuestSaveEntry>();

    public GameData()
    {
        this.timeCount = 0;
        this.Health = 100; 
        this.CurrentWeapon = null;
        this.IsInVehicle = false;
        this.currentSceneName = "MainScene"; 


        this.playerPosition = new Vector3(-111f, -130f, 0f);
        this.CurrentMagazineAmmo = 0;
        this.TotalReserveAmmo = 0;
        this.WeaponSlots = new List<string>();
        this.ActiveWeaponIndex = -1;
        this.quests = new List<QuestSaveEntry>();
    }
}
