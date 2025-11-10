using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DisplaySetting : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown displayOption;
    public Toggle FullScreen;

    private Resolution[] allResolutions;
    private List<Resolution> validResolutions = new List<Resolution>();
    private bool isFullScreen;
    private int selectedResolution;

    private void Start()
    {
        // 🟦 Khởi tạo fullscreen thật của hệ thống
        isFullScreen = true;
        FullScreen.isOn = isFullScreen;

        // 🟦 Lấy danh sách độ phân giải hỗ trợ
        allResolutions = Screen.resolutions;
        List<string> resolutionStringList = new List<string>();
        HashSet<string> used = new HashSet<string>();

        foreach (Resolution res in allResolutions)
        {
            string key = $"{res.width}x{res.height}";
            if (used.Add(key)) // tránh trùng width/height
            {
                float refreshRate = (float)res.refreshRateRatio.value;
                string formatted = $"{res.width}x{res.height}@{Mathf.RoundToInt(refreshRate)}Hz";
                resolutionStringList.Add(formatted);
                validResolutions.Add(res);
            }
        }

        // 🟦 Cập nhật dropdown
        displayOption.ClearOptions();
        displayOption.AddOptions(resolutionStringList);

        // 🟦 Tìm độ phân giải mặc định 4K (nếu có)
        selectedResolution = validResolutions.FindIndex(r => r.width == 3840 && r.height == 2160);
        if (selectedResolution == -1)
            selectedResolution = validResolutions.Count - 1; // fallback: max res

        displayOption.value = selectedResolution;
        displayOption.RefreshShownValue();

        // 🟦 Đặt game về 4K UHD khi khởi chạy
        ApplyResolution();
    }

    public void ChangeDisplay()
    {
        selectedResolution = displayOption.value;
        ApplyResolution();
    }

    public void ChangeFullScreen()
    {
        isFullScreen = FullScreen.isOn;
        ApplyResolution();
    }

    private void ApplyResolution()
    {
        if (selectedResolution < 0 || selectedResolution >= validResolutions.Count)
            return;

        Resolution res = validResolutions[selectedResolution];

        // ✅ Chuyển từ bool sang FullScreenMode
        FullScreenMode mode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        // ✅ Gọi SetResolution đúng tham số
        Screen.SetResolution(res.width, res.height, mode, (int)res.refreshRateRatio.value);

        Debug.Log($"Resolution set: {res.width}x{res.height}, mode={mode}, refresh={res.refreshRateRatio.value}");
    }
}
