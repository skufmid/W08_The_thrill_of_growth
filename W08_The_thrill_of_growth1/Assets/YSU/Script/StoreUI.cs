using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class StoreUI : MonoBehaviour
{
    private const int GRID_SIZE = 9;
    private const int MAX_LEVEL = 30;
    private const int GRID_ROWS = 3;
    private const int GRID_COLS = 3;
    private const int MAX_CHARACTER_ID = 15; // 최대 캐릭터 ID 범위

    private const int defaultRestPrice = 100;

    private static StoreUI instance;
    public static StoreUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StoreUI>();
                if (instance == null)
                {
                    GameObject go = new GameObject("StoreUI");
                    instance = go.AddComponent<StoreUI>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    [Header("UI References")]
    [SerializeField] private Button storeButton;
    [SerializeField] private Button readyButton;  // 준비완료 버튼
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI partyText;  // 파티 인원 텍스트
    [SerializeField] private Button[] levelButtons = new Button[GRID_SIZE];
    [SerializeField] private float dragOffset = 1f;  // 드래그 시 캐릭터가 띄워질 높이
    [SerializeField] private Button sellModeButton; // 판매 모드 버튼

    [Header("Character Settings")]
    [SerializeField] private Character characterPrefab;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private int sellPrice = 50;
    [SerializeField] private int baseLevelUpPrice = 100;  // 기본 레벨업 가격
    [SerializeField] private int levelUpPriceIncrease = 50;  // 레벨당 증가하는 가격

    private Button[] characterSlots;
    private Button[] sellButtons;
    private Button[] restButtons;
    private PartyManager partyManager;
    private PlayerData playerData;
    private bool isDragging = false;
    private Character selectedCharacter = null;
    private int selectedSlotIndex = -1;
    private bool isStoreOpen = false;
    private bool isReady = false;  // 준비완료 상태

    private EStoreMode storeMode; // 상점의 모드

    private Character draggedCharacter = null;
    private int dragStartSlot = -1;
    private Vector3 originalPosition;
    private Vector3 mouseOffset;    // 마우스와 캐릭터 간의 오프셋

    [Header("Store Panel")]
    [SerializeField] private GameObject storePanel;

    private void Start()
    {
        InitializeManagers();
        InitializeUI();
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

        playerData.OnGoldChanged += UpdateGoldUI;
    }

    private void InitializeUI()
    {
        Transform slotCanvas = GameObject.Find("SlotCanvas")?.transform;
        Transform Buybuttons = GameObject.Find("BuyButton")?.transform;
        if (slotCanvas == null)
        {
            Debug.LogError("SlotCanvas를 찾을 수 없습니다!");
            enabled = false;
            return;
        }
        if(Buybuttons == null)
        {
            Debug.LogError("BuyButton을 찾을 수 없습니다!");
            enabled = false;
            return;
        }

        InitializeButtons(slotCanvas, Buybuttons);
        SetupButtonListeners();
        UpdateGoldUI(playerData.Gold);
    }

    private void InitializeButtons(Transform canvas, Transform Buybutton)
    {
        Debug.Log("StoreUI 초기화 시작");   
        characterSlots = new Button[GRID_SIZE];
        sellButtons = new Button[GRID_SIZE];
        restButtons = new Button[GRID_SIZE];

        for (int i = 0; i < GRID_SIZE; i++)
        {
            int row = (i / GRID_COLS) + 1;
            int col = (i % GRID_COLS) + 1;

            // 구매 버튼 초기화
            string buyButtonName = $"{row}_{col} Buy";
            characterSlots[i] = Buybutton.Find(buyButtonName)?.GetComponent<Button>();
            if (characterSlots[i] == null) Debug.LogError($"{buyButtonName}을 찾을 수 없습니다!");

            // 판매 버튼 초기화
            string sellButtonName = $"{row}_{col} Sell";
            sellButtons[i] = Buybutton.Find(sellButtonName)?.GetComponent<Button>();
            if (sellButtons[i] == null) Debug.LogError($"{sellButtonName}을 찾을 수 없습니다!");

            // 치료 버튼 초기화
            string restButtonName = $"{row}_{col} Rest";
            restButtons[i] = Buybutton.Find(restButtonName)?.GetComponent<Button>();
            if (restButtons[i] == null) Debug.LogError($"{restButtonName}을 찾을 수 없습니다!");

            // 레벨업 버튼 검증
            if (levelButtons[i] == null) Debug.LogError($"레벨업 버튼 {i + 1}이 인스펙터에 할당되지 않았습니다!");
        }

        ValidateSpawnPositions();
    }

    private void ValidateSpawnPositions()
    {
        if (spawnPositions == null || spawnPositions.Length == 0)
        {
            spawnPositions = new Transform[GRID_SIZE];
            for (int i = 0; i < GRID_SIZE; i++)
            {
                int row = (i / GRID_COLS) + 1;
                int col = (i % GRID_COLS) + 1;
                string posName = $"Pos{row}_{col}";
                spawnPositions[i] = GameObject.Find(posName)?.transform;
                if (spawnPositions[i] == null) Debug.LogError($"{posName}을 찾을 수 없습니다!");
            }
        }
    }

    private void SetupButtonListeners()
    {
        storeButton.onClick.AddListener(ToggleStore);
        readyButton.onClick.AddListener(OnReadyClicked);

        // 초기에 준비완료 버튼 비활성화
        readyButton.gameObject.SetActive(false);

        for (int i = 0; i < GRID_SIZE; i++)
        {
            int slotIndex = i;
            if (characterSlots[i] != null)
            {
                characterSlots[i].onClick.AddListener(() => OnCharacterSlotClicked(slotIndex));
                characterSlots[i].gameObject.SetActive(false);
            }

            if (levelButtons[i] != null)
            {
                levelButtons[i].onClick.AddListener(() => OnLevelUpClicked(slotIndex));
                levelButtons[i].gameObject.SetActive(false);
            }

            if (sellButtons[i] != null)
            {
                sellButtons[i].onClick.AddListener(() => OnSellClicked(slotIndex));
                sellButtons[i].gameObject.SetActive(false);
            }

            if (restButtons[i] != null)
            {
                restButtons[i].onClick.AddListener(() => OnRestClicked(slotIndex));
                restButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnCharacterSlotClicked(int slotIndex)
    {
        if (!CanPurchaseCharacter())
            return;

        if (TrySpawnCharacter(slotIndex, out Character newCharacter))
        {
            if (partyManager.PurchaseAndAddCharacter(newCharacter, slotIndex))
            {
                UpdateAllSlotsUI();
                UpdatePartyText();  // 파티 텍스트 업데이트 추가
                Debug.Log($"슬롯 {slotIndex}에 캐릭터를 구매했습니다.");
            }
            else
            {
                Destroy(newCharacter.gameObject);
            }
        }
    }

    private bool CanPurchaseCharacter()
    {
        if (partyManager.IsPartyFull())
        {
            ShowWarningMessage("파티가 가득 찼습니다!");
            return false;
        }

        if (!playerData.HasEnoughGold(partyManager.GetCharacterPrice()))
        {
            ShowWarningMessage("골드가 부족합니다!");
            return false;
        }

        return true;
    }

    private bool TrySpawnCharacter(int slotIndex, out Character character)
    {
        character = null;
        if (slotIndex >= 0 && slotIndex < spawnPositions.Length && spawnPositions[slotIndex] != null)
        {
            character = Instantiate(characterPrefab, spawnPositions[slotIndex].position, Quaternion.Euler(0, 180, 0));

            // 현재 보유한 캐릭터들의 ID 목록 가져오기
            List<int> existingIds = new List<int>();
            for (int i = 0; i < GRID_SIZE; i++)
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
                newId = Random.Range(0, MAX_CHARACTER_ID + 1);  // 0부터 시작하도록 수정
            } while (existingIds.Contains(newId));

            // 생성된 캐릭터에 ID 설정
            character.Id = newId;
            Debug.Log($"새로운 캐릭터 생성: ID {newId}");  // 디버그 로그 추가

            return true;
        }
        return false;
    }

    private void OnLevelUpClicked(int slotIndex)
    {
        Character character = partyManager.GetCharacterAtSlot(slotIndex);
        if (character != null && character.Level < MAX_LEVEL)
        {
            int currentLevelUpPrice = baseLevelUpPrice + (character.Level - 1) * levelUpPriceIncrease;

            if (playerData.HasEnoughGold(currentLevelUpPrice))
            {
                if (playerData.SpendGold(currentLevelUpPrice))
                {
                    character.LevelUp();
                    Debug.Log($"슬롯 {slotIndex}의 캐릭터가 레벨업 했습니다. 현재 레벨: {character.Level}, 소모 골드: {currentLevelUpPrice}");
                }
            }
            else
            {
                Debug.Log($"레벨업에 필요한 골드가 부족합니다. 필요 골드: {currentLevelUpPrice}");
            }
        }
    }

    private void OnSellClicked(int slotIndex)
    {
        Character character = partyManager.GetCharacterAtSlot(slotIndex);
        if (character != null)
        {
            Destroy(character.gameObject);

            if (partyManager.RemoveCharacter(slotIndex))
            {
                playerData.AddGold(sellPrice);
                UpdateSlotUI(slotIndex);
                UpdatePartyText();  // 파티 텍스트 업데이트 추가
                Debug.Log($"슬롯 {slotIndex}의 캐릭터가 판매되었습니다. 획득한 골드: {sellPrice}");
            }
        }
    }

    private void OnRestClicked(int slotIndex)
    {
        Character character = partyManager.GetCharacterAtSlot(slotIndex);
        if (character != null && character.Hp < character.MaxHp)
        {
            int restPrice = defaultRestPrice;

            if (playerData.HasEnoughGold(restPrice))
            {
                if (playerData.SpendGold(restPrice))
                {
                    character.RestoreFullly();
                }
            }
            else
            {
                Debug.Log($"휴식에 필요한 골드가 부족합니다. 필요 골드: {restPrice}");
            }
        }
    }

    private void UpdateGoldUI(int currentGold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {currentGold}";
        }
    }

    private void ToggleStore()
    {
        isStoreOpen = !isStoreOpen;
        storeButton.gameObject.SetActive(!isStoreOpen);
        readyButton.gameObject.SetActive(isStoreOpen);  // 상점이 열리면 준비완료 버튼 활성화
        isReady = false;  // 상점을 열 때마다 준비 상태 초기화
        if (storePanel != null)
            storePanel.SetActive(isStoreOpen); // 상점 패널 표시/숨김
        UpdateAllSlotsUI();
    }

    private void UpdateAllSlotsUI()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            UpdateSlotUI(i);
        }
        UpdatePartyText();  // 파티 텍스트 업데이트 추가
    }

    private void UpdateSlotUI(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < characterSlots.Length)
        {
            bool isOccupied = partyManager.IsSlotOccupied(slotIndex);
            Character character = partyManager.GetCharacterAtSlot(slotIndex);

            UpdateButtonVisibility(slotIndex, isOccupied, character);
        }
    }

    private void UpdateButtonVisibility(int slotIndex, bool isOccupied, Character character)
    {
        // 준비완료 상태이면 모든 버튼 비활성화
        if (isReady)
        {
            if (characterSlots[slotIndex] != null) characterSlots[slotIndex].gameObject.SetActive(false);
            if (levelButtons[slotIndex] != null) levelButtons[slotIndex].gameObject.SetActive(false);
            if (sellButtons[slotIndex] != null) sellButtons[slotIndex].gameObject.SetActive(false);
            return;
        }

        if (characterSlots[slotIndex] != null)
        {
            characterSlots[slotIndex].gameObject.SetActive(isStoreOpen && !isOccupied && storeMode == EStoreMode.Hire);
        }

        if (levelButtons[slotIndex] != null)
        {
            levelButtons[slotIndex].gameObject.SetActive(isStoreOpen && isOccupied && character != null && character.Level < MAX_LEVEL && storeMode == EStoreMode.Hire);
        }

        if (sellButtons[slotIndex] != null)
        {
            // 판매 모드일 때만 판매 버튼 활성화
            sellButtons[slotIndex].gameObject.SetActive(isStoreOpen && isOccupied && storeMode == EStoreMode.Sell);
        }

        // 이곳에 회복 버튼 추가
    }

    private void ShowWarningMessage(string message)
    {
        Debug.Log(message);
    }

    private void Update()
    {
        if (isReady) return;  // 준비완료 상태에서는 드래그 불가능

        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치에서 레이캐스트
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                Character character = hit.collider.GetComponent<Character>();
                if (character != null)
                {
                    // 드래그 시작
                    draggedCharacter = character;
                    dragStartSlot = GetSlotIndexFromPosition(character.transform.position);
                    originalPosition = character.transform.position;

                    // 마우스와 캐릭터 위치의 차이를 저장
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = character.transform.position.z;
                    mouseOffset = character.transform.position - mousePos;

                    // 캐릭터를 약간 위로 띄움
                    character.transform.position += Vector3.forward * dragOffset;
                }
            }
        }

        // 드래그 중
        if (draggedCharacter != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = draggedCharacter.transform.position.z;
            draggedCharacter.transform.position = mousePos + mouseOffset;
        }

        // 드래그 종료
        if (Input.GetMouseButtonUp(0) && draggedCharacter != null)
        {
            // 마우스 위치에서 가장 가까운 슬롯 찾기
            int targetSlot = GetNearestSlotIndex(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (targetSlot != -1 && targetSlot != dragStartSlot)
            {
                // 슬롯 교환 시도
                if (TrySwapCharacters(dragStartSlot, targetSlot))
                {
                    Debug.Log($"캐릭터를 슬롯 {dragStartSlot}에서 {targetSlot}로 이동했습니다.");
                }
                else
                {
                    // 교환 실패 시 원위치
                    draggedCharacter.transform.position = originalPosition;
                }
            }
            else
            {
                // 유효하지 않은 위치면 원위치
                draggedCharacter.transform.position = originalPosition;
            }

            // 드래그 상태 초기화
            draggedCharacter = null;
            dragStartSlot = -1;
        }
    }

    private int GetSlotIndexFromPosition(Vector3 position)
    {
        // 가장 가까운 스폰 위치의 인덱스 반환
        float minDistance = float.MaxValue;
        int nearestIndex = -1;

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            float distance = Vector3.Distance(position, spawnPositions[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private int GetNearestSlotIndex(Vector3 position)
    {
        return GetSlotIndexFromPosition(position);
    }

    private bool TrySwapCharacters(int fromSlot, int toSlot)
    {
        if (fromSlot == toSlot) return false;

        Character fromChar = partyManager.GetCharacterAtSlot(fromSlot);
        Character toChar = partyManager.GetCharacterAtSlot(toSlot);

        // 파티 매니저에서 캐릭터 교환
        if (partyManager.SwapCharacters(fromSlot, toSlot))
        {
            // 위치 교환
            Vector3 fromPos = spawnPositions[fromSlot].position;
            Vector3 toPos = spawnPositions[toSlot].position;

            // 전투 라인 위치 계산 및 설정
            if (fromChar != null)
            {
                fromChar.transform.position = toPos;
                fromChar.position = GetLinePositionForSlot(toSlot);
            }
            if (toChar != null)
            {
                toChar.transform.position = fromPos;
                toChar.position = GetLinePositionForSlot(fromSlot);
            }

            // UI 업데이트
            UpdateSlotUI(fromSlot);
            UpdateSlotUI(toSlot);

            return true;
        }

        return false;
    }

    // 슬롯 인덱스에 따른 전투 라인 위치 반환
    private CombatLine.linePosition GetLinePositionForSlot(int slotIndex)
    {
        // 슬롯 인덱스를 열로 변환 (0,3,6: Back, 1,4,7: Middle, 2,5,8: Front)
        int col = slotIndex % GRID_COLS;

        switch (col)
        {
            case 0:
                return CombatLine.linePosition.Back;
            case 1:
                return CombatLine.linePosition.Middle;
            case 2:
                return CombatLine.linePosition.Front;
            default:
                return CombatLine.linePosition.None;
        }
    }

    private void OnReadyClicked()
    {
        isReady = true;
        readyButton.gameObject.SetActive(false);  // 준비완료 버튼 비활성화
        storeButton.gameObject.SetActive(false);  // 상점 버튼 비활성화
        isStoreOpen = false;  // 상점 상태를 닫힘으로 설정
        if (storePanel != null)
            storePanel.SetActive(false); // 상점 패널 숨김
        UpdateAllSlotsUI();  // 모든 버튼 상태 업데이트
        Manager.Game.StartStage(); // 스테이지 시작
        Debug.Log("준비 완료! 스테이지 시작!");
    }

    // 상점 UI 활성화
    public void ShowStore()
    {
        storeButton.gameObject.SetActive(true);
        isReady = false;  // 준비 상태 초기화
        isStoreOpen = false; // 상점은 닫힌 상태로 시작
        readyButton.gameObject.SetActive(false);  // 준비완료 버튼 초기 비활성화
        UpdateAllSlotsUI();
    }

    // 게임 재시작이나 새 라운드 시작 시 호출할 메서드
    public void ResetReadyState()
    {
        isReady = false;
        isStoreOpen = false;
        storeButton.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(false);  // 초기 상태에서는 준비완료 버튼 비활성화
        UpdateAllSlotsUI();
    }

    // 외부에서 호출 가능한 파티 텍스트 업데이트 메서드
    public void UpdatePartyTextExternal()
    {
        UpdatePartyText();
    }

    private void UpdatePartyText()
    {
        if (partyText != null)
        {
            int currentParty = partyManager.GetCurrentPartySize();
            partyText.text = $"파티 : {currentParty}/{PartyManager.MAX_PARTY_SIZE}";
        }
    }

    public void ToggleStoreMode(EStoreMode storeMode)
    {
        this.storeMode = storeMode;
        UpdateAllSlotsUI();
    }
}