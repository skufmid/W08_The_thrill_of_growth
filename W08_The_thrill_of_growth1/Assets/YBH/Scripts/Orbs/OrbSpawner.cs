using System.Collections.Generic;
using UnityEngine;


public class OrbSpawner : MonoBehaviour
{
    public static OrbSpawner Instance;

    [Header("Drop Settings")]
    [SerializeField] private OrbDropEntry[] orbDropEntries; // 오브 드랍 데이터들
    private Canvas _orbCanvas;
    private RectTransform _forbiddenArea;

    private void Awake()
    {
        Instance = this;
        _orbCanvas = GetComponentInChildren<Canvas>();

        GameObject forbiddenAreaObject = GameObject.Find("ForbiddenArea");
        if (forbiddenAreaObject != null)
        {
            _forbiddenArea = forbiddenAreaObject.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("❗ 'ForbiddenArea'라는 이름을 가진 GameObject를 찾을 수 없습니다.");
        }
    }
    public void SpawnOrbsOnDeath(Vector3 screenPosition, float potionChance, float itemChance, float uniqueChance)
    {
        TrySpawnCategory(OrbCategory.Potion, screenPosition, potionChance);
        TrySpawnCategory(OrbCategory.Item, screenPosition, itemChance);
        TrySpawnCategory(OrbCategory.Unique, screenPosition, uniqueChance);
    }

    private void TrySpawnCategory(OrbCategory category, Vector3 screenPosition, float dropChance)
    {
        List<OrbDropEntry> entries = GetEntriesByCategory(category);
        if (entries.Count == 0) return;

        if (Random.value <= dropChance)
        {
            OrbDropEntry selected = GetRandomOrbEntry(entries);
            if (selected != null)
            {
                SpawnOrb(selected, screenPosition);
            }
        }
    }

    private List<OrbDropEntry> GetEntriesByCategory(OrbCategory category)
    {
        List<OrbDropEntry> result = new List<OrbDropEntry>();
        foreach (var entry in orbDropEntries)
        {
            if (entry.category == category)
                result.Add(entry);
        }
        return result;
    }

    private OrbDropEntry GetRandomOrbEntry(List<OrbDropEntry> entries)
    {
        float total = 0f;
        foreach (var entry in entries)
            total += entry.spawnRate;

        float roll = Random.Range(0f, total);
        float current = 0f;
        foreach (var entry in entries)
        {
            current += entry.spawnRate;
            if (roll <= current)
                return entry;
        }

        return null;
    }

    private void SpawnOrb(OrbDropEntry selected, Vector3 screenPosition)
    {
        GameObject orbGO = Instantiate(selected.prefab, _orbCanvas.transform);

        RectTransform rt = orbGO.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _orbCanvas.transform as RectTransform,
            screenPosition,
            null, // Overlay 모드일 경우 카메라 필요 없음
            out localPoint
        );
        rt.anchoredPosition = localPoint;

        Orb orb = orbGO.GetComponent<Orb>();
        if (orb != null)
        {
            orb.orbType = selected.type;
            orb.value = selected.useFixedValue
                ? selected.fixedValue
                : Mathf.Floor(Random.Range(selected.minRandomValue, selected.maxRandomValue) * 10f) / 10f;
            orb.forbiddenArea = _forbiddenArea;
            orb.canvasParent = _orbCanvas;
        }
        else
        {
            Debug.LogError("❗ Spawn된 오브에 Orb 컴포넌트가 없습니다.");
        }
    }
    public void SpawnRandomOrb(Vector3 screenPosition)
    {
        OrbDropEntry selected = GetRandomOrbEntry();
        if (selected.prefab == null)
        {
            Debug.LogError($"❗ {selected.type} 프리팹이 설정되지 않았습니다.");
            return;
        }
        float value = selected.useFixedValue
        ? selected.fixedValue
    :   Mathf.Floor(Random.Range(selected.minRandomValue, selected.maxRandomValue) * 10f) / 10f;

        // 프리팹 생성
        GameObject orbGO = Instantiate(selected.prefab, _orbCanvas.transform); // 오브 캔버스에 소환
        Orb orbGOsorb = orbGO.GetComponent<Orb>();
        orbGOsorb.forbiddenArea = _forbiddenArea; // 스포너에서 오브로 넘겨줌
        orbGOsorb.canvasParent = _orbCanvas; // 스포너에서 오브로 넘겨줌
        // 바로 스크린 좌표로 이동
        RectTransform rt = orbGO.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _orbCanvas.transform as RectTransform,
            screenPosition,
            null, // Overlay는 Camera가 필요 없음
            out localPoint
        );
        rt.anchoredPosition = localPoint;

        Orb orb = orbGO.GetComponentInChildren<Orb>();
        if (orb != null)
        {
            orb.orbType = selected.type;
            orb.value = value;
        }
        else
        {
            Debug.LogError("❗ Orb 컴포넌트를 찾을 수 없습니다.");
        }
        if (_forbiddenArea != null)
        {
            orb.forbiddenArea = _forbiddenArea; // 스포너에서 오브로 넘겨줌
        }

    }

    private OrbDropEntry GetRandomOrbEntry()
    {
        float total = 0f;
        foreach (var entry in orbDropEntries)
            total += entry.spawnRate;

        float roll = Random.Range(0f, total);
        float current = 0f;

        foreach (var entry in orbDropEntries)
        {
            if(orbDropEntries.Length == 0)
            {
                Debug.LogError("❗ 오브 드랍 엔트리가 비어있습니다.");
                return entry;
            }
            current += entry.spawnRate;
            if (roll <= current)
                return entry;
        }

        return orbDropEntries[0]; // fallback
    }
}