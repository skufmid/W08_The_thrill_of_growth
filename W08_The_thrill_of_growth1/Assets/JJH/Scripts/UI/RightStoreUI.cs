using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightStoreUI : MonoBehaviour
{
    private PartyManager partyManager;
    private PlayerData playerData;
    private StoreUI storeUI;

    public Image mercenarySprite;
    public TextMeshProUGUI allianceText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI mercenaryInfoText;


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
        int characterId = SetRandomMercenary();
        int orignalPrice = CalCulateOriginalPrice();
        int changedPrice = (int)(orignalPrice * 0.8f);

        CharacterSO character = Array.Find(Manager.Data.Charaters, c => c.Id == characterId);
        Debug.Log(Manager.Data.CharaterSprites.Length);
        Debug.Log(characterId);
        mercenarySprite.sprite = Manager.Data.CharaterSprites[characterId];
        allianceText.text = SynergyManager.SynergyTypeToKorean[character.SynergyType];
        classText.text = SynergyManager.CharacterTypeToKorean[character.CharacterType];
        mercenaryInfoText.text = @$"Lv 00\t{character.Name}
Gold: <s><i>{orignalPrice}</i></s> <b><size=46>→</size> <color=#FF4040>{changedPrice}</b></color>";
    }

    private int CalCulateOriginalPrice()
    {
        return 999;
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
        int characterId;
        do
        {
            characterId = UnityEngine.Random.Range(0, StoreUI.MAX_CHARACTER_ID + 1);  // 0부터 시작하도록 수정
        } while (existingIds.Contains(characterId));

        // 생성된 캐릭터에 ID 설정
        return characterId;
    }
}
