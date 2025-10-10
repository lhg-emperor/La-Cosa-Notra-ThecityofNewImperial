using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string hoverParam; // VD: "Story", "Quit", ...
    private MenuAnimatorController controller;

    void Start()
    {
        controller = GetComponentInParent<MenuAnimatorController>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.OnButtonHover(hoverParam);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        controller.OnButtonExit();
    }
}
