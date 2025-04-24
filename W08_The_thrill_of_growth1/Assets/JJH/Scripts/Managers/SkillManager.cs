using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SkillManager : MonoBehaviour
{

    private static SkillManager _instance;
    public static SkillManager Instance => _instance;

    SkillComponent skillComponent;

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

    public void InvokeSkill(Unit unit, int skillId)
    {
        SkillSO skill = Array.Find(Manager.Data.Skills, s => s.Id == skillId);
        if (skillId >= 100) return;


        Debug.Log($"Character {unit.Id} 스킬 실행");
        Character character = unit.GetComponent<Character>();
        coefficient = Manager.Data.Skills[skillId].Coefficients[character.Star];
        switch (skillId)
        {
            case 0:
                value = unit.MaxHp;
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(1), value * coefficient / 100);
                break;

            case 1:
                float HpRatio = unit.Hp / unit.MaxHp;
                if (HpRatio < 0.8f)
                {
                    coefficient = 1.00f;
                }
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(1), unit.Damage * coefficient / 100);
                break;

            case 2:
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(3), unit.Damage * coefficient / 100);
                break;

            case 3:
                GameObject[] Targets = Manager.Battle.characterList.ToArray();
                skillComponent.ApplyEffectEnemyPercentSkill(Targets, EStat.AttackSpeed, null, EStat.AttackSpeed, coefficient);
                break;

            case 4:
                Targets = new GameObject[1];
                Targets[0] = unit.gameObject;
                value = unit.MaxHp;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Hp, null, value * coefficient / 100);
                break;

            case 5:
                skillComponent.RepeatBasicAttack(character, Manager.Battle.GetRandomEnemy(1)[0], 3, coefficient / 100);
                break;

            case 6:
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(1), unit.Damage * coefficient / 100);
                break;

            case 7:
                break;

            case 8:
                value = unit.Damage;
                Targets = Manager.Battle.characterList.ToArray();
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Damage, null, value * coefficient / 100);
                skillComponent.ApplySelfDamage(unit.gameObject, 5f / 100); // 자해
                break;

            case 9:
                value = unit.MaxHp - unit.Hp;
                Targets = new GameObject[1];
                Targets[0] = unit.gameObject;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Damage, null, value * coefficient / 100);
                skillComponent.ApplySelfDamage(unit.gameObject, 5f / 100); // 자해
                break;

            case 10:
                value = unit.Damage;
                Targets = Manager.Battle.enemyList.ToArray();
                skillComponent.DamageSkill(Targets, value * coefficient / 100);
                skillComponent.ApplySelfDamage(unit.gameObject, 5f / 100); // 자해
                break;

            case 11:
                break;

            case 12:
                value = unit.Damage;
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(1), value * coefficient / 100);
                break;

            case 13:
                Targets = Manager.Battle.characterList.ToArray();
                skillComponent.ApplyEffectEnemyPercentSkill(Targets, EStat.Damage, null, EStat.Damage, coefficient);
                break;

            case 14:
                Targets = Manager.Battle.characterList.ToArray();
                value = unit.Damage;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.Hp, null, value * coefficient / 100);
                break;

            case 15:
                Targets = new GameObject[1];
                Targets[0] = unit.gameObject;
                value = unit.Damage;
                skillComponent.ApplyEffectAmountSkill(Targets, EStat.AttackSpeed, null, value * coefficient / 100);
                break;

        }
    }

    public void InvokeEnemySkill(Enemy enemy)
    {
        Debug.Log($"Enemy {enemy.Id} 스킬 실행");
        value = enemy.Damage;
        skillComponent.DamageSkill(new GameObject[] { Manager.Battle.GetTargetByPositionPriority() }, value);
    }
}
