using UnityEngine;
using UnityEngine.EventSystems;
using static SynergyManager;

public class AllianceImageHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SynergyType synergyType; // 설정할 세력 타입

    public void OnPointerEnter(PointerEventData eventData)
    {
        string desc = SynergyManager.GetSynergyDescription(synergyType);
        TooltipManager.Instance?.Show(desc, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance?.Hide();
    }
}