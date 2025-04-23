using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    public static BattleManager Instance { get; private set; } //싱글톤
    public List<GameObject> characterList;      //캐릭터 리스트
    public List<GameObject> enemyList;         //적 리스트
    public SynergyManager synergyManager;

    public void Init()
    {
        characterList = new List<GameObject>();
        enemyList = new List<GameObject>();
    }
    public void AddCharacter(GameObject character)  //리스트에 캐릭터 추가
    {
        characterList.Add(character);
    }
    public void AddEnemy(GameObject enemy)      //리스트에 적 추가
    {
        enemyList.Add(enemy);
    }
    public void CheckSynergies()
    {
        synergyManager.EvaluateSynergies(characterList);
    }
    public GameObject GetTargetByPositionPriority() //적 우선순위 시스템
    {
        CombatLine.linePosition[] priority = new CombatLine.linePosition[]
        {
        CombatLine.linePosition.Taunt,
        CombatLine.linePosition.Front,
        CombatLine.linePosition.Middle,
        CombatLine.linePosition.Back
        };

        // 라인 우선순위 순회
        for (int i = 0; i < priority.Length; i++)
        {
            CombatLine.linePosition currentLine = priority[i];

            // 캐릭터 리스트 순회
            for (int j = 0; j < characterList.Count; j++)
            {
                GameObject character = characterList[j];
                Character ch = character.GetComponent<Character>();

                if (ch != null && ch.position == currentLine)
                {
                    return character; // 첫 타겟 찾으면 바로 반환
                }
            }
        }

        return null; // 타겟 없음
    }


}
