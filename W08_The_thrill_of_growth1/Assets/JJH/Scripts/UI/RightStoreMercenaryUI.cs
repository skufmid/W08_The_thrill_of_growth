using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RightStoreMercenaryUI : MonoBehaviour
{
    private PartyManager partyManager;
    private PlayerData playerData;
    private StoreUI storeUI;

    public Image mercenarySprite;
    public TextMeshProUGUI allianceText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI mercenaryInfoText;

    int characterId;
    int level;

    private void Awake()
    {
        storeUI = FindAnyObjectByType<StoreUI>();
        InitializeManagers();
    }
    private void Start()
    {

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
    public void OnMercenaryBuyClicked()
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

        if (TrySpawnMercenary(slotIndex, out Character newCharacter, characterId, level))
        {
            if (partyManager.PurchaseAndAddCharacter(newCharacter, slotIndex))
            {
                storeUI.UpdateAllSlotsUI();
                storeUI.UpdatePartyTextExternal();
                ClearPurchaseableMercenary();
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

        if (characterId < 0 || characterId > StoreUI.MAX_CHARACTER_ID)
        {
            Debug.Log($"Invalid character ID: {characterId}");
            return false;
        }

        // 이미 존재하는 캐릭터라면 생성하지 않음
        List<int> existingIds = new List<int>();
        for (int i = 0; i < partyManager.partySlots.Length; i++)
        {
            Character existingChar = partyManager.GetCharacterAtSlot(i);
            if (existingChar != null)
            {
                existingIds.Add(existingChar.Id);
            }
        }

        if (existingIds.Contains(characterId)) return false;

        if (slotIndex >= 0 && slotIndex < storeUI.spawnPositions.Length && storeUI.spawnPositions[slotIndex] != null)
        {
            character = Instantiate(storeUI.characterPrefab, storeUI.spawnPositions[slotIndex].position, Quaternion.Euler(0, 180, 0));
            character.Id = charaterId;
            character.Level = level;
            return true;
        }
        return false;
    }

    public void DisplayPurchaseableMercenary()
    {
        characterId = SetRandomMercenary();
        int orignalPrice = CalCulateOriginalPrice();
        int changedPrice = (int)(orignalPrice * 0.8f);

        CharacterSO character = Array.Find(Manager.Data.Charaters, c => c.Id == characterId);
        Debug.Log(characterId);
        mercenarySprite.sprite = Manager.Data.CharaterSprites[characterId];
        mercenarySprite.color = Color.white;
        allianceText.text = SynergyManager.SynergyTypeToKorean[character.SynergyType];
        classText.text = SynergyManager.CharacterTypeToKorean[character.CharacterType];
        mercenaryInfoText.text = @$"Lv 00\t{character.Name}
Gold: <s><i>{orignalPrice}</i></s> <b><size=46>→</size> <color=#FF4040>{changedPrice}</b></color>";
    }

    private void ClearPurchaseableMercenary()
    {
        characterId = -1;
        mercenarySprite.sprite = null;
        mercenarySprite.color = Color.clear;
        allianceText.text = string.Empty;
        classText.text = string.Empty;
        mercenaryInfoText.text = string.Empty;
    }

    private int CalCulateOriginalPrice()
    {
        return 1000;
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
