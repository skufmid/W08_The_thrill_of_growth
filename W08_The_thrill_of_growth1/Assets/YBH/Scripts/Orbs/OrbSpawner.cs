using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public static OrbSpawner Instance;

    [SerializeField] private OrbDropEntry[] orbDropEntries;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnRandomOrb(Vector3 worldPosition)
    {
        OrbDropEntry selected = GetRandomOrbEntry();

        if (selected.prefab == null)
        {
            Debug.LogError($"❗ {selected.type} 프리팹이 설정되지 않았습니다.");
            return;
        }

        float value = selected.useFixedValue
            ? selected.fixedValue
            : Random.Range(selected.minRandomValue, selected.maxRandomValue);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        GameObject orbGO = Instantiate(selected.prefab);
        orbGO.transform.position = screenPos;

        Orb orb = orbGO.GetComponent<Orb>();
        orb.orbType = selected.type;
        orb.value = value;
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
            current += entry.spawnRate;
            if (roll <= current)
                return entry;
        }

        return orbDropEntries[0]; // fallback
    }
}