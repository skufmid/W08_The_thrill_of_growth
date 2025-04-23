using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SynergySlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;

    public void OnPointerEnter(PointerEventData eventData)
    {

        Manager.Tooltip.Show(description, Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Manager.Tooltip.Hide();
    }
}