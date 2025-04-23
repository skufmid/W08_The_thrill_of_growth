using UnityEngine;

public class SynergeTestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Test", 3f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Test()
    {
        if (Manager.Synergy == null)
        {
            Debug.LogError("❗ Manager.Synergy가 null입니다.");
        }
        else if (BattleManager.Instance.characterList == null)
        {
            Debug.LogError("❗ characterList가 null입니다.");
        }
        else if (BattleManager.Instance.characterList.Count == 0)
        {
            Debug.LogWarning("⚠️ characterList에 캐릭터가 없음.");
        }
        Manager.Synergy.EvaluateSynergies(Manager.Battle.characterList);
        Debug.Log("EvaluateSynergies");
    }
}
