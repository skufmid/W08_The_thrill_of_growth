using UnityEngine;



public enum OrbCategory
{
    Potion,
    Item,
    Unique
}
[System.Serializable]
public class OrbDropEntry
{
    public OrbCategory category;
    public OrbType type;
    public GameObject prefab;
    public float spawnRate;        // 확률 (총합 100 기준이든 비율이든 가능)
    public bool useFixedValue;     // 고정값 사용할지 여부
    public float fixedValue;       // 고정값
    public float minRandomValue;   // 랜덤 최소값
    public float maxRandomValue;   // 랜덤 최대값
}