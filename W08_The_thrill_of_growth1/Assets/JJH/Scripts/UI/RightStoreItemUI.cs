using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class RightStoreItemUI : MonoBehaviour
{
    public GameObject ItemPanels;
    public Canvas OrbCanvas;
    public RectTransform ForbiddenArea;
    public GameObject[] ItemPrefabs;
    private ItemUI[] Items;
    private PlayerData playerData;
    private void Awake()
    {
        playerData = PlayerData.Instance;

        Items = ItemPanels.GetComponentsInChildren<ItemUI>();
        for (int i = 0; i < ItemPrefabs.Length; i++)
        {
            Instantiate(ItemPrefabs[i], Items[i].transform.GetChild(0).transform);
            Button buyButton = Items[i].GetComponentInChildren<Button>();
            int index = i;
            buyButton.onClick.AddListener(() => OnBuyButtonClicked(index));
            Items[i].InitUI(i);
        }
        for (int i = ItemPrefabs.Length; i < Items.Length; i++)
        {
            Items[i].gameObject.SetActive(false);
        }
    }

    private void OnBuyButtonClicked(int index)
    {
        int price = ItemPrefabs[index].GetComponent<Orb>().price;
        if (playerData.HasEnoughGold(price))
        {
            if (playerData.SpendGold(price))
            {
                GameObject orbGO = Instantiate(ItemPrefabs[index], OrbCanvas.transform);
                Vector3 randomPosition = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                orbGO.transform.localPosition += randomPosition + Vector3.up * 200f;

                Orb orb = orbGO.GetComponent<Orb>();
                orb.forbiddenArea = ForbiddenArea; // 스포너에서 오브로 넘겨줌
                orb.canvasParent = OrbCanvas; // 스포너에서 오브로 넘겨줌
            }
        }
        else
        {
            Debug.Log($"아이템 구매에 필요한 골드가 부족합니다. 필요 골드: {price}");
        }
    }
}
