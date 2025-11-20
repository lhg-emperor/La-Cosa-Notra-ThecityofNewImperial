using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("Weapon UI")]
    public Image weaponIconUI;
    public TextMeshProUGUI magazineSizeText;
    public TextMeshProUGUI magazineCountText;

    [Header("Weapon Sprites")]
    public Sprite defaultIcon;


    public void UpdateWeaponDisplay(Sprite newIcon, int currentMag, int totalReserve)
    {
        if (weaponIconUI != null)
            weaponIconUI.sprite = newIcon;

        if (magazineSizeText != null)
        {
            magazineSizeText.text = currentMag.ToString();
        }

        if (magazineCountText != null)
            magazineCountText.text = totalReserve.ToString();
    }
}