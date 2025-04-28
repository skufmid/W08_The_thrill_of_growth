using UnityEngine;
using UnityEngine.EventSystems;
using static SynergyManager;

public class ClassImageHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CharacterType characterType; // 설정할 직업 타입

    public void OnPointerEnter(PointerEventData eventData)
    {
        string desc = SynergyManager.GetClassDescription(characterType);
        TooltipManager.Instance?.Show(desc, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance?.Hide();
    }
}