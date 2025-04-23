using System.Collections;
using UnityEngine;

public class SynergyDebugTester : MonoBehaviour
{
    [Header("키 입력으로 시너지 재평가")]
    public KeyCode triggerKey = KeyCode.R;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f); // 매니저 초기화 기다림

        Debug.Log("✅ SynergyDebugTester 준비 완료");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Manager.Battle.characterList == null || Manager.Battle.characterList.Count == 0)
            {
                Debug.LogError("❗ characterList가 비어 있거나 null입니다.");
                return;
            }
            Manager.Synergy.ResetAndReevaluateSynergies(Manager.Battle.characterList);
        }
    }
}