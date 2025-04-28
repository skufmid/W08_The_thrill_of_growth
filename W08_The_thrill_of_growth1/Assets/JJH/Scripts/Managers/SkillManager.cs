using System;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    private static SkillManager _instance;
    public static SkillManager Instance => _instance;

    SkillComponent skillComponent;

    public GameObject SelfDamageFX;

    float value;
    float coefficient;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
    }
    public void Init()
    {
        skillComponent = GetComponent<SkillComponent>();
    }

    private void SpawnSkillFX(GameObject SkillFX, GameObject[] Targets)
    {
        if (Targets.Length == 0)
        {
            Debug.LogError($"⚠️ {SkillFX.name}의 타겟이 없습니다!");
        }
        foreach (GameObject Target in Targets)
        {
            Instantiate(SkillFX, Target.transform.position + SkillFX.transform.position, Quaternion.identity);
        }
    }

    public void InvokeSkill(Unit unit, int skillId)
    {
        SkillSO skill = Array.Find(Manager.Data.Skills, s => s.Id == skillId);
        if (skillId >= 100) return;
        GameObject[] Targets = new GameObject[0];

        Debug.Log($"Character {unit.Id} 스킬 실행");
        Character character = unit.GetComponent<Character>();
        coefficient = skill.Coefficients[character.Star];
        switch (skillId)
        {
            case 0:
                value = unit.MaxHp;
                Targets = Manager.Battle.GetRandomEnemy(1);
                skillComponent.DamageSkill(Targets, unit, value * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 1:
                float HpRatio = unit.Hp / unit.MaxHp;
                if (HpRatio < 0.8f)
                {
                    coefficient = 100f;
                }
                Targets = Manager.Battle.GetRandomEnemy(1);
                skillComponent.DamageSkill(Targets, unit, unit.Damage * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 2:
                Targets = Manager.Battle.GetRandomEnemy(3);
                skillComponent.DamageSkill(Targets, unit, unit.Damage * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 3:
                Targets = Manager.Battle.characterList.ToArray();
                skillComponent.ApplyEffectEnemyPercentSkill(Targets, EStat.AttackSpeed, null, EStat.AttackSpeed, coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 4:
                Targets = new GameObject[] { unit.gameObject };
                value = unit.MaxHp;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Hp, null, value * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 5:
                Targets = Manager.Battle.GetRandomEnemy(1);
                skillComponent.RepeatBasicAttack(character, Targets[0], 3, coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 6:
                Targets = Manager.Battle.GetRandomEnemy(1);
                skillComponent.DamageSkill(Targets, unit, unit.Damage * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 7:
                Debug.Log("스킬 7로 효과 없음");
                break;

            case 8:
                value = unit.Damage;
                Targets = Manager.Battle.characterList.ToArray();
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Damage, null, value * coefficient / 100);
                skillComponent.ApplySelfDamage(unit.gameObject, 5f / 100); // 자해
                SpawnSkillFX(skill.skillPrefab, Targets);
                SpawnSkillFX(SelfDamageFX, new[] { unit.gameObject });
                break;

            case 9:
            case 11:
                value = unit.MaxHp - unit.Hp;
                Targets = new GameObject[1];
                Targets[0] = unit.gameObject;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Damage, null, value * coefficient / 100);
                skillComponent.ApplySelfDamage(unit.gameObject, 5f / 100); // 자해
                SpawnSkillFX(skill.skillPrefab, Targets);
                SpawnSkillFX(SelfDamageFX, new[] { unit.gameObject });
                break;

            case 10:
                value = unit.Damage;
                Targets = Manager.Battle.enemyList.ToArray();
                skillComponent.DamageSkill(Targets, unit, value * coefficient / 100);
                skillComponent.ApplySelfDamage(unit.gameObject, 5f / 100); // 자해
                SpawnSkillFX(skill.skillPrefab, Targets);
                SpawnSkillFX(SelfDamageFX, new[] { unit.gameObject });
                break;

            case 12:
                value = unit.Damage;
                Targets = Manager.Battle.GetRandomEnemy(1);
                skillComponent.DamageSkill(Targets, unit, value * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 13:
                Targets = Manager.Battle.characterList.ToArray();
                skillComponent.ApplyEffectEnemyPercentSkill(Targets, EStat.Damage, null, EStat.Damage, coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 14:
                Targets = Manager.Battle.characterList.ToArray();
                value = unit.Damage;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Hp, null, value * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

            case 15:
                Targets = new GameObject[1];
                Targets[0] = unit.gameObject;
                value = unit.Damage;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.AttackSpeed, null, value * coefficient / 100);
                SpawnSkillFX(skill.skillPrefab, Targets);
                break;

        }
    }

    public void InvokeEnemySkill(Enemy enemy)
    {
        Debug.Log($"Enemy {enemy.Id} 스킬 실행");
        value = enemy.Damage;
        skillComponent.DamageSkill(new GameObject[] { Manager.Battle.GetTargetByPositionPriority() }, enemy, value);
        
    }
}
