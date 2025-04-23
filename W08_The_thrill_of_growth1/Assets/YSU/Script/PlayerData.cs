using UnityEngine;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour
{
    private static PlayerData instance;
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerData>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PlayerData");
                    instance = go.AddComponent<PlayerData>();
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
        DontDestroyOnLoad(gameObject);
        
        // 게임 매니저의 스테이지 승리 이벤트 구독
        Manager.Game.OnEndStage += OnStageCleared;
    }

    [SerializeField] private int gold = 1000; // 시작 골드
    [SerializeField] private int baseStageReward = 100; // 기본 스테이지 보상
    [SerializeField] private int stageRewardIncrease = 50; // 스테이지당 증가하는 보상량
    
    public int Gold
    {
        get { return gold; }
        set { 
            gold = value;
            OnGoldChanged?.Invoke(gold);
        }
    }

    // 골드 변경 이벤트
    public delegate void GoldChangedHandler(int newGold);
    public event GoldChangedHandler OnGoldChanged;

    // 스테이지 클리어 보상
    private void OnStageCleared()
    {
        int currentStage = Manager.Game.stageNum;
        int reward = baseStageReward + (currentStage - 1) * stageRewardIncrease;
        AddGold(reward);
        Debug.Log($"Stage {currentStage} Cleared! Reward: {reward} Gold");
    }

    // 골드 획득
    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            Gold += amount;
        }
    }

    // 골드 사용
    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;
        
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }

    // 골드가 충분한지 확인
    public bool HasEnoughGold(int amount)
    {
        return Gold >= amount;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (Manager.Game != null)
        {
            Manager.Game.OnEndStage -= OnStageCleared;
        }
    }
} 