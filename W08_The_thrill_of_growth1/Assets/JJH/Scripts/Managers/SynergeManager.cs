using System.Collections.Generic;
using UnityEngine;
using static Character;

public class SynergyManager
{
    public enum SynergyType { Kingdom, Northward, Dark, HolyLight }
    public enum CharacterType { Tanker, Warrior, Wizard, Archer }
    public void EvaluateSynergies(List<GameObject> characterList)
    {
        Dictionary<SynergyType, int> synergyCounts = new Dictionary<SynergyType, int>();
        Dictionary<CharacterType, int> typeCounts = new Dictionary<CharacterType, int>();
        Dictionary<CombatLine.linePosition, int> lineCounts = new Dictionary<CombatLine.linePosition, int>();

        foreach (GameObject characterObj in characterList)
        {
            Character character = characterObj.GetComponent<Character>();
            if (character == null) continue;

            // 시너지 타입 카운트
            if (!synergyCounts.ContainsKey(character.synergyType))
                synergyCounts[character.synergyType] = 0;
            synergyCounts[character.synergyType]++;

            // 캐릭터 타입 카운트
            if (!typeCounts.ContainsKey(character.characterType))
                typeCounts[character.characterType] = 0;
            typeCounts[character.characterType]++;

            // 라인 포지션 카운트
            if (!lineCounts.ContainsKey(character.position))
                lineCounts[character.position] = 0;
            lineCounts[character.position]++;
        }

        // 조건 체크 예시
        if (synergyCounts.TryGetValue(SynergyType.Kingdom, out int kingdomCount) && kingdomCount >= 2)
        {
            ActivateFireSynergy();
        }

        if (lineCounts.TryGetValue(CombatLine.linePosition.Taunt, out int tauntCount) && tauntCount >= 1)
        {
            ActivateFrontlineSynergy();
        }

        // 등등...
    }

    private void ActivateFireSynergy()
    {
        Debug.Log("🔥 Fire 시너지 발동!");
    }

    private void ActivateFrontlineSynergy()
    {
        Debug.Log("🛡️ 전열 시너지 발동!");
    }
}