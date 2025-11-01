using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class DisplaySetting : MonoBehaviour
{
    public TMP_Dropdown displayOption;
    public Toggle FullScreen;

    Resolution[] allResolutions;
    bool isFullScreen;
    int selectedResolution;
    List<Resolution> SelectedList = new List<Resolution>();

    private void Start()
    {
        isFullScreen = true;
        allResolutions = Screen.resolutions;
        
        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in allResolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();
            if(!resolutionStringList.Contains(newRes))
            {
             float refreshRate = (float)res.refreshRateRatio.value;
             string formatted = $"{res.width}x{res.height}@{Mathf.RoundToInt(refreshRate)}Hz";
             resolutionStringList.Add(formatted);
             SelectedList.Add(res);

            }

        }
        displayOption.ClearOptions();
        displayOption.AddOptions(resolutionStringList);
    }
    public void ChangeDisPlay()
    {
        selectedResolution = displayOption.value;
        Screen.SetResolution(SelectedList[selectedResolution].width, SelectedList[selectedResolution].height, isFullScreen);
    }
    public void ChangeFullScreen()
    {
        isFullScreen = FullScreen.isOn;
        Screen.SetResolution(SelectedList[selectedResolution].width, SelectedList[selectedResolution].height, isFullScreen);
    }

}
