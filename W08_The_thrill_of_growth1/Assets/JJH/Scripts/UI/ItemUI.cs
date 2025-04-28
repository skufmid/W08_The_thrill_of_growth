using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    Orb orb;
    TextMeshProUGUI itemGoldText;

    public void InitUI(int i)
    {
        orb = GetComponentInChildren<Orb>();
        itemGoldText = GetComponentInChildren<TextMeshProUGUI>();

        itemGoldText.text = "Gold: " + orb.price.ToString();
    }



}
