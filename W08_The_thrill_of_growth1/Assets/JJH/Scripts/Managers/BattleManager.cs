using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    public static BattleManager Instance { get; private set; } //싱글톤
    public List<GameObject> characterList;      //캐릭터 리스트
    public List<GameObject> enemyList;         //적 리스트
    public SynergyManager synergyManager;
    public bool isInBattle;


    public void Init()
    {
        characterList = new List<GameObject>();
        enemyList = new List<GameObject>();
    }
    public void AddCharacter(GameObject character)  //리스트에 캐릭터 추가
    {
        characterList.Add(character);
        Debug.Log($"Add {Manager.Battle.characterList.Count}");
    }
    public void AddEnemy(GameObject enemy)      //리스트에 적 추가
    {
        enemyList.Add(enemy);
    }

    public void RemoveCharacter(GameObject character) // 리스트에서 캐릭터 제거
    {
        if (characterList.Contains(character))
        {
            characterList.Remove(character);
            Debug.Log($"Remove {Manager.Battle.characterList.Count}");

        }

        if (characterList.Count == 0 && isInBattle)
        {
            Manager.Game.DefeatStage();
        }
    }
    public void RemoveEnemy(GameObject enemy) // 리스트에서 적 제거
    {
        if (enemyList.Contains(enemy))
        {
            enemyList.Remove(enemy);
        }

        if (enemyList.Count == 0 && isInBattle)
        {
            Manager.Game.WinStage();
        }
    }

    public void CheckSynergies()
    {
        synergyManager.EvaluateSynergies(characterList);
    }
    public GameObject GetTargetByPositionPriority()
    {
        CombatLine.linePosition[] priority = new CombatLine.linePosition[]
        {
        CombatLine.linePosition.Taunt,
        CombatLine.linePosition.Front,
        CombatLine.linePosition.Middle,
        CombatLine.linePosition.Back
        };

        for (int i = 0; i < priority.Length; i++)
        {
            CombatLine.linePosition currentLine = priority[i];

            // 해당 라인의 캐릭터들을 모은다
            List<GameObject> candidates = new List<GameObject>();

            for (int j = 0; j < characterList.Count; j++)
            {
                GameObject character = characterList[j];
                Character ch = character.GetComponent<Character>();

                if (ch != null && ch.position == currentLine)
                {
                    candidates.Add(character);
                }
            }

            // 후보가 있으면 랜덤하게 선택해서 반환
            if (candidates.Count > 0)
            {
                int randIndex = Random.Range(0, candidates.Count);
                return candidates[randIndex];
            }
        }

        return null; // 타겟 없음
    }

    public GameObject[] GetRandomEnemy(int enemyNum)
    {
        // 요청 수가 리스트 크기보다 크면, 가능한 최대치만 반환
        int count = Mathf.Min(enemyNum, enemyList.Count);

        // 리스트 복사
        List<GameObject> temp = new List<GameObject>(enemyList);

        // Fisher–Yates shuffle
        for (int i = 0; i < count; i++)
        {
            // i부터 끝까지 중 하나를 뽑아 swap
            int randIndex = UnityEngine.Random.Range(i, temp.Count);
            GameObject tmp = temp[i];
            temp[i] = temp[randIndex];
            temp[randIndex] = tmp;
        }

        // 앞에서 count개를 배열로 잘라 반환
        return temp.GetRange(0, count).ToArray();
    }
}
