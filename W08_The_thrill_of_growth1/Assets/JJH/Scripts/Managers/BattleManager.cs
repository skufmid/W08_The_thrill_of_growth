using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    public static BattleManager Instance { get; private set; } //싱글톤
    public List<GameObject> characterList;      //캐릭터 리스트
    public List<GameObject> enemyList;         //적 리스트

    public void Init()
    {
        characterList = new List<GameObject>();
        enemyList = new List<GameObject>();
    }
    public void AddCharacter(GameObject character)
    {
        characterList.Add(character);
    }
    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
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
