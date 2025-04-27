using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RightStoreUI : MonoBehaviour
{
    private PartyManager partyManager;
    private PlayerData playerData;
    private StoreUI storeUI;

    private void Awake()
    {
        storeUI = FindAnyObjectByType<StoreUI>();
    }
    private void Start()
    {
        InitializeManagers();
        DisplayPurchaseableMercenary();
    }

    private void InitializeManagers()
    {
        partyManager = PartyManager.Instance;
        playerData = PlayerData.Instance;

        if (partyManager == null || playerData == null)
        {
            Debug.LogError("필수 매니저를 찾을 수 없습니다!");
            enabled = false;
            return;
        }
    }
    private void OnMercenaryBuyClicked(int charaterId, int level)
    {
        if (!CanPurchaseMercenary())
            return;

        int slotIndex = -1;
        for (int i = 0; i < StoreUI.GRID_SIZE; i++)
        {
            if (!partyManager.partySlots[i].isOccupied)
            {
                slotIndex = i;
                break;
            }
        }

        if (TrySpawnMercenary(slotIndex, out Character newCharacter, charaterId, level))
        {
            if (partyManager.PurchaseAndAddCharacter(newCharacter, slotIndex))
            {
                storeUI.UpdateAllSlotsUI();
                storeUI.UpdatePartyTextExternal();
            }
            else
            {
                Destroy(newCharacter.gameObject);
            }
        }
    }

    private bool CanPurchaseMercenary()
    {
        return true;
    }

    private bool TrySpawnMercenary(int slotIndex, out Character character, int charaterId, int level)
    {
        character = null;
        if (slotIndex >= 0 && slotIndex < storeUI.spawnPositions.Length && storeUI.spawnPositions[slotIndex] != null)
        {
            character = Instantiate(storeUI.characterPrefab, storeUI.spawnPositions[slotIndex].position, Quaternion.Euler(0, 180, 0));
            character.Id = charaterId;
            character.Level = level;
            return true;
        }
        return false;
    }

    private void DisplayPurchaseableMercenary()
    {
        int newId = SetRandomMercenary();
        int price = CalCulateOriginalPrice();


    }

    private int CalCulateOriginalPrice()
    {
        return 0;
    }

    private int SetRandomMercenary()
    {
        List<int> existingIds = new List<int>();
        for (int i = 0; i < partyManager.partySlots.Length; i++)
        {
            Character existingChar = partyManager.GetCharacterAtSlot(i);
            if (existingChar != null)
            {
                existingIds.Add(existingChar.Id);
            }
        }

        // 중복되지 않는 랜덤 ID 생성 (0부터 시작)
        int newId;
        do
        {
            newId = Random.Range(0, StoreUI.MAX_CHARACTER_ID + 1);  // 0부터 시작하도록 수정
        } while (existingIds.Contains(newId));

        // 생성된 캐릭터에 ID 설정
        return newId;
    }
}
