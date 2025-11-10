using UnityEngine;

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

    public GameData()
    {
        this.timeCount = 0;
        this.Health = 100; // giá trị mặc định
        this.CurrentWeapon = null;
        this.IsInVehicle = false;
        this.currentSceneName = "MainScene"; // giá trị mặc định nếu chưa lưu
    }
}
