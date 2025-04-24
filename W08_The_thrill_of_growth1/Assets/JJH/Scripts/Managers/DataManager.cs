using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SynergyManager;

public class DataManager
{
    public CharacterSO[] Charaters;
    public SkillSO[] Skills;
    public EnemySO[] Enemies;
    public SynergyDataSO[] synergyDataList;
    public CharacterDataSO[] characterDataList;
    public Dictionary<SynergyType, Sprite> synergyIcon;
    public Dictionary<CharacterType, Sprite> characterIcon;
    public Dictionary<SynergyManager.CharacterType, GameObject> projectileMap;
    public GameObject[] projectilePrefabs; // 인덱스: CharacterType 순서
    public void Init()
    {
        Charaters = Resources.LoadAll<CharacterSO>("Characters");
        Skills = Resources.LoadAll<SkillSO>("Skills");
        Enemies = Resources.LoadAll<EnemySO>("Enemies");
        synergyDataList = Resources.LoadAll<SynergyDataSO>("Synergy");
        characterDataList = Resources.LoadAll<CharacterDataSO>("CharacterSynergy");
        // 딕셔너리 초기화
        synergyIcon = new Dictionary<SynergyType, Sprite>();
        characterIcon = new Dictionary<CharacterType, Sprite>();

        // 자동 매핑
        foreach (var synergy in synergyDataList)
        {
            if (!synergyIcon.ContainsKey(synergy.synergyType))
                synergyIcon.Add(synergy.synergyType, synergy.icon);
        }

        foreach (var character in characterDataList)
        {
            if (!characterIcon.ContainsKey(character.charactertType))
                characterIcon.Add(character.charactertType, character.icon);
        }
        //ShowAll();
        // Projectile Map 초기화
        projectileMap = new Dictionary<SynergyManager.CharacterType, GameObject>();
        GameObject[] projectilePrefabs = Resources.LoadAll<GameObject>("Projectiles");

        foreach (GameObject prefab in projectilePrefabs)
        {
            foreach (SynergyManager.CharacterType type in System.Enum.GetValues(typeof(SynergyManager.CharacterType)))
            {
                if (prefab.name.ToLower().Contains(type.ToString().ToLower()))
                {
                    if (!projectileMap.ContainsKey(type))
                    {
                        projectileMap.Add(type, prefab);
                    }
                }
            }
        }
        foreach (SynergyManager.CharacterType type in System.Enum.GetValues(typeof(SynergyManager.CharacterType)))
        {
            if (!projectileMap.ContainsKey(type))
            {
                Debug.LogWarning($"⚠️ {type}용 프리팹이 등록되지 않았습니다.");
            }
        }
    }

    public void ShowAll()
    {
        foreach (var character in Charaters)
        {
            //Debug.Log($"이름: {character.Name}, 클래스: {character.Class}, HP: {character.BaseHP}");
        }
        
        foreach (var skill in Skills)
        {
            //Debug.Log($"이름: {skill.Name}, 설명: {skill.SkillDescription}");
        }

        foreach (var character in Enemies)
        {
            //Debug.Log($"이름: {character.Name}, HP: {character.BaseHP}");
        }
        foreach (var synergy in synergyDataList)
        {
            //Debug.LogError($"시너지: {synergy.synergyType}, 아이콘: {synergy.icon.name}");
        }

        foreach (var character in characterDataList)
        {
            //Debug.LogError($"시너지2: {character.charactertType}, 설명: {character.icon.name}");
        }

    }
    public Sprite GetCharacterIcon(CharacterType type)   // 캐릭터 아이콘 가져오기
    {
        if (characterIcon.TryGetValue(type, out var icon))
            return icon;

        Debug.LogWarning($"[DataManager] 캐릭터 아이콘이 없습니다: {type}");
        return null;
    }

    public Sprite GetSynergyIcon(SynergyType type)       // 시너지 아이콘 가져오기
    {
        if (synergyIcon.TryGetValue(type, out var icon))
            return icon;

        Debug.LogWarning($"[DataManager] 시너지 아이콘이 없습니다: {type}");
        return null;
    }
}
