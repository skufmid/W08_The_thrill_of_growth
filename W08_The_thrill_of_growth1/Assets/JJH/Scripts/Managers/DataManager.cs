using UnityEngine;

public class DataManager
{
    public CharacterSO[] Charaters;
    public SkillSO[] Skills;
    public EnemySO[] Enemies;

    public void Init()
    {
        Charaters = Resources.LoadAll<CharacterSO>("Characters");
        Skills = Resources.LoadAll<SkillSO>("Skills");
        Enemies = Resources.LoadAll<EnemySO>("Enemies");

        ShowAll();
    }

    public void ShowAll()
    {
        foreach (var character in Charaters)
        {
            Debug.Log($"이름: {character.Name}, 클래스: {character.Class}, HP: {character.BaseHP}");
        }
        
        foreach (var skill in Skills)
        {
            Debug.Log($"이름: {skill.Name}, 설명: {skill.SkillDescription}");
        }

        foreach (var character in Enemies)
        {
            Debug.Log($"이름: {character.Name}, HP: {character.BaseHP}");
        }
    }
}
