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
    }

    [SerializeField] private int gold = 1000; // 시작 골드
    
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
} 