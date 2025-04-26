using UnityEngine;


public class OrbSpawner : MonoBehaviour
{
    public static OrbSpawner Instance;
    public RectTransform forbiddenArea; // 금지구역
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
    :   Mathf.Floor(Random.Range(selected.minRandomValue, selected.maxRandomValue) * 10f) / 10f;

        // 프리팹 생성
        GameObject orbGO = Instantiate(selected.prefab);

        // 월드 공간 캔버스에 직접 위치 지정
        orbGO.transform.position = worldPosition;

        //// 캔버스에 넣기
        //Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        //orbGO.transform.SetParent(canvas.transform, true); // true: worldPosition 유지

        // Orb 컴포넌트 설정
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
        if (forbiddenArea != null)
        {
            //orb.forbiddenArea = forbiddenArea; // 스포너에서 오브로 넘겨줌
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