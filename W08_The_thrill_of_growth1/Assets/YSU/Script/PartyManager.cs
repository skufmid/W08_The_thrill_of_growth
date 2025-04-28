using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    private static PartyManager instance;
    public static PartyManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managers = GameObject.Find("Managers");
                if (managers != null)
                {
                    instance = managers.GetComponentInChildren<PartyManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("PartyManager");
                        go.transform.SetParent(managers.transform);
                        instance = go.AddComponent<PartyManager>();
                    }
                }
                else
                {
                    Debug.LogError("Managers 오브젝트를 찾을 수 없습니다!");
                }
            }
            return instance;
        }
    }

    public const int MAX_PARTY_SIZE = 5;
    public const int TOTAL_SLOTS = 9;  // 전체 슬롯 수
    [SerializeField] private int characterPrice = 100; // 캐릭터 기본 가격
    [SerializeField] private int characterPriceIncrease = 100; // 캐릭터 가격 증가
    private PlayerData playerData;

    [System.Serializable]
    public class PartySlot
    {
        public Character character;
        public CombatLine.linePosition position;
        public bool isOccupied;

        public PartySlot()
        {
            character = null;
            position = CombatLine.linePosition.None;
            isOccupied = false;
        }
    }

    [SerializeField]
    public PartySlot[] partySlots;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // 파티 슬롯 배열 초기화
        if (partySlots == null || partySlots.Length != TOTAL_SLOTS)
        {
            partySlots = new PartySlot[TOTAL_SLOTS];
            InitializePartySlots();
        }

        // PlayerData 찾기 시도
        if (playerData == null)
        {
            playerData = PlayerData.Instance;
            if (playerData == null)
            {
                Debug.LogError("PlayerData를 찾을 수 없습니다! PlayerData 오브젝트가 씬에 있는지 확인해주세요.");
            }
        }
    }

    private void Start()
    {
        // PlayerData가 없다면 다시 한번 찾기 시도
        if (playerData == null)
        {
            playerData = PlayerData.Instance;
        }
    }

    private void InitializePartySlots()
    {
        Debug.Log($"파티 슬롯 초기화 시작 (크기: {TOTAL_SLOTS})");
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            if (partySlots[i] == null)
            {
                partySlots[i] = new PartySlot();
            }
            Debug.Log($"슬롯 {i} 초기화 완료");
        }
    }

    // 캐릭터 구매 및 추가
    public bool PurchaseAndAddCharacter(Character character, int slotIndex)
    {
        // PlayerData가 없다면 다시 한번 찾기 시도
        if (playerData == null)
        {
            playerData = PlayerData.Instance;
            if (playerData == null)
            {
                Debug.LogError("PlayerData not found!");
                return false;
            }
        }

        // 슬롯 유효성 검사
        if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS)
        {
            Debug.LogWarning("Invalid slot index!");
            return false;
        }

        // 슬롯이 이미 사용중인지 확인
        if (partySlots[slotIndex].isOccupied)
        {
            Debug.LogWarning("Slot is already occupied!");
            return false;
        }

        // 파티가 가득 찼는지 확인
        if (IsPartyFull())
        {
            Debug.LogWarning("Party is full!");
            return false;
        }

        // 골드 확인 및 차감
        if (!playerData.SpendGold(characterPrice))
        {
            Debug.LogWarning("Not enough gold!");
            return false;
        }

        // 캐릭터 추가
        return AddCharacter(character, slotIndex);
    }

    // 캐릭터 추가 (내부 사용)
    private bool AddCharacter(Character character, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS)
            return false;

        if (partySlots[slotIndex].isOccupied)
            return false;

        partySlots[slotIndex].character = character;

        CombatLine.linePosition pos = CombatLine.linePosition.Back;
        switch (slotIndex % 3)
        {
            case 2:
                pos = CombatLine.linePosition.Front;
                break;

            case 1:
                pos = CombatLine.linePosition.Middle;
                break;

            case 0:
                pos = CombatLine.linePosition.Back;
                break;
        }
        character.position = pos;
        partySlots[slotIndex].position = character.position;
        partySlots[slotIndex].isOccupied = true;
        
        return true;
    }

    // 캐릭터 제거
    public bool RemoveCharacter(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS)
            return false;

        if (!partySlots[slotIndex].isOccupied)
            return false;

        Character character = partySlots[slotIndex].character;
        Manager.Battle.RemoveCharacter(character.gameObject);

        partySlots[slotIndex].character = null;
        partySlots[slotIndex].position = CombatLine.linePosition.None;
        partySlots[slotIndex].isOccupied = false;

        return true;
    }

    // 캐릭터로 슬롯 찾아서 제거
    public bool RemoveCharacterByReference(Character character)
    {
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            if (partySlots[i].character == character)
            {
                return RemoveCharacter(i);
            }
        }
        return false;
    }

    // 특정 포지션의 캐릭터들 가져오기
    public List<Character> GetCharactersByPosition(CombatLine.linePosition position)
    {
        List<Character> characters = new List<Character>();
        foreach (var slot in partySlots)
        {
            if (slot.isOccupied && slot.position == position)
            {
                characters.Add(slot.character);
            }
        }
        return characters;
    }

    // 모든 파티 캐릭터 가져오기
    public List<Character> GetAllCharacters()
    {
        List<Character> characters = new List<Character>();
        foreach (var slot in partySlots)
        {
            if (slot.isOccupied)
            {
                characters.Add(slot.character);
            }
        }
        return characters;
    }

    // 현재 파티 멤버 수 반환
    public int GetCurrentPartySize()
    {
        int count = 0;
        foreach (var slot in partySlots)
        {
            if (slot.isOccupied)
                count++;
        }
        return count;
    }

    // 파티가 가득 찼는지 확인
    public bool IsPartyFull()
    {
        return GetCurrentPartySize() >= MAX_PARTY_SIZE;
    }

    // 현재 캐릭터 가격 반환
    public int GetCharacterPrice()
    {
        return characterPrice;
    }

    // 캐릭터 구매가 가능한지 확인
    public bool CanPurchaseCharacter()
    {
        return playerData != null && !IsPartyFull() && playerData.HasEnoughGold(characterPrice);
    }

    // 특정 슬롯이 사용중인지 확인
    public bool IsSlotOccupied(int slotIndex)
    {
        // 배열 범위 체크
        if (partySlots == null)
        {
            Debug.LogError("파티 슬롯 배열이 초기화되지 않았습니다!");
            return true;
        }

        if (slotIndex < 0 || slotIndex >= partySlots.Length)
        {
            Debug.LogError($"잘못된 슬롯 인덱스: {slotIndex}, 최대 슬롯: {partySlots.Length}");
            return true;
        }

        if (partySlots[slotIndex] == null)
        {
            Debug.LogError($"슬롯 {slotIndex}가 null입니다!");
            return true;
        }

        return partySlots[slotIndex].isOccupied;
    }

    // 특정 슬롯의 캐릭터 가져오기
    public Character GetCharacterAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS || partySlots[slotIndex] == null)
        {
            return null;
        }
        return partySlots[slotIndex].character;
    }

    // 캐릭터 위치 교환
    public bool SwapCharacters(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= TOTAL_SLOTS || 
            toIndex < 0 || toIndex >= TOTAL_SLOTS ||
            partySlots[fromIndex] == null || partySlots[toIndex] == null)
        {
            return false;
        }

        // 캐릭터와 위치 정보 교환
        Character tempChar = partySlots[fromIndex].character;
        CombatLine.linePosition tempPos = partySlots[fromIndex].position;
        bool tempOccupied = partySlots[fromIndex].isOccupied;

        partySlots[fromIndex].character = partySlots[toIndex].character;
        partySlots[fromIndex].position = partySlots[toIndex].position;
        partySlots[fromIndex].isOccupied = partySlots[toIndex].isOccupied;

        partySlots[toIndex].character = tempChar;
        partySlots[toIndex].position = tempPos;
        partySlots[toIndex].isOccupied = tempOccupied;

        return true;
    }
} 