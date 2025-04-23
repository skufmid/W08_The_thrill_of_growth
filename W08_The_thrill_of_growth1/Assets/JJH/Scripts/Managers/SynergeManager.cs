using System.Collections;
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

            //// 라인 포지션 카운트 (지금 사용 안함)
            //if (!lineCounts.ContainsKey(character.position))
            //    lineCounts[character.position] = 0;
            //lineCounts[character.position]++;
        }



        //-------- 직업 시너지 ------------------
        // 탱커 시너지
        if (typeCounts.TryGetValue(CharacterType.Tanker, out int tankCount))
        {
            if (tankCount >= 4) ApplyHpBuff(0.40f);
            else if (tankCount >= 3) ApplyHpBuff(0.20f);
            else if (tankCount >= 2) ApplyHpBuff(0.10f);
        }
        // 전사 시너지
        if (typeCounts.TryGetValue(CharacterType.Warrior, out int warriorCount))
        {
            if (warriorCount >= 4) ApplyVampiricBuff(0.20f);
            else if (warriorCount >= 3) ApplyVampiricBuff(0.10f);
            else if (warriorCount >= 2) ApplyVampiricBuff(0.05f);
        }
        // 마법사 시너지
        if (typeCounts.TryGetValue(CharacterType.Wizard, out int wizardCount))
        {
            if (wizardCount >= 4) ApplyManaBuff(4f);
            else if (wizardCount >= 3) ApplyManaBuff(2f);
            else if (wizardCount >= 2) ApplyManaBuff(1f);
        }
        // 궁수 시너지
        if (typeCounts.TryGetValue(CharacterType.Archer, out int archerCount))
        {
            if (archerCount >= 4) ApplyAttackSpeedBuff(0.40f);
            else if (archerCount >= 3) ApplyAttackSpeedBuff(0.20f);
            else if (archerCount >= 2) ApplyAttackSpeedBuff(10f);
        }


        //-------- 세력 시너지 ------------------
        // 왕국 연합
        if (synergyCounts.TryGetValue(SynergyType.Kingdom, out int kingdomCount))
        {
            if (kingdomCount >= 4) ApplyAttackBuff(0.40f);
            else if (kingdomCount >= 3) ApplyAttackBuff(0.20f);
            else if (kingdomCount >= 2) ApplyAttackBuff(0.10f);
        }
        //북방 부족


        if (synergyCounts.TryGetValue(SynergyType.Northward, out int northCount)) // 공격 1회 추가공격으로 수정해야함.
        {
            if (northCount >= 4) ApplyAttackBuff(0.40f);
            else if (northCount >= 3) ApplyAttackBuff(0.20f);
            else if (northCount >= 2) ApplyAttackBuff(0.10f);
        }
        //어둠의 교단
        if (synergyCounts.TryGetValue(SynergyType.Dark, out int darkCount))
        {
            if (darkCount >= 4) ApplyAttackBuff(0.50f);
        }
        //성광 교단
        if (synergyCounts.TryGetValue(SynergyType.HolyLight, out int holyCount))
        {
            if (holyCount >= 4) ApplyAttackBuff(0.40f);
            else if (holyCount >= 3) ApplyAttackBuff(0.20f);
            else if (holyCount >= 2) ApplyAttackBuff(0.10f);
        }
    }

    private void ApplyHpBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null || ch.characterType != CharacterType.Tanker) continue;

            ch.MaxHp *= 1 + ratio;
            ch.Hp = ch.MaxHp; 
            Debug.Log("🛡️ 전열 시너지 발동!"+ch.name+ ratio);
        }
    }

    private void ApplyAttackBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null|| ch.characterType != CharacterType.Warrior) continue;

            ch.Damage *= 1 + ratio;
        }
    }
    private void ApplyAttackSpeedBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null) continue;
            ch.AttackSpeed *= 1 + ratio;
            Debug.Log("🛡️ 궁수 시너지 발동!");

        }
    }
    private void ApplyVampiricBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null) continue;
            ch.Vampiric *= 1 + ratio;
        }
    }
    private void ApplyManaBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null) continue;
            ch.manaGain = 10 + ratio;
        }
    }

    IEnumerator LightHeal(float healPercent, float flat)
    {
        yield return new WaitForSeconds(5f);
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null) continue;
            ch.Hp += ch.MaxHp * healPercent + flat;
            Debug.Log("🛡️ 성광 시너지 발동!");
        }
    }
}