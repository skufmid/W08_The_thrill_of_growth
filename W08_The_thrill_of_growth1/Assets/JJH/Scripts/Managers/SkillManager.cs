using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SkillManager : MonoBehaviour
{
    SkillComponent skillComponent;
    float value;
    float coefficient;
    public void Init()
    {
        skillComponent = GetComponent<SkillComponent>();
    }

    public void InvokeSkill(Unit unit, int skillId)
    {
    

        SkillSO skill = Array.Find(Manager.Data.Skills, s => s.Id == skillId);
        if (skillId >= 100) return;

        Character character = unit.GetComponent<Character>();
        //coefficient = skill.
        switch (skillId)
        {
            case 0:
                value = unit.MaxHp;
                switch (character.Star)
                {
                    case 1:
                        coefficient = 0.3f;
                        break;
                    case 2:
                        coefficient = 0.35f;
                        break;
                    case 3:
                        coefficient = 0.4f;
                        break;
                    case 4:
                        coefficient = 0.45f;
                        break;
                }
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(1), value * coefficient);
                break;

            case 1:
                float HpRatio = unit.Hp / unit.MaxHp;
                if (HpRatio >= 0.8f)
                {
                    //coefficient
                }
                else
                {
                    coefficient = 1.00f;
                }
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(1), unit.Damage * coefficient);
                break;

            case 2:
                //coeffiecint
                skillComponent.DamageSkill(Manager.Battle.GetRandomEnemy(3), unit.Damage * coefficient);
                break;

            case 3:
                //coeffiecient

                GameObject[] Targets = Manager.Battle.characterList.ToArray();
                skillComponent.ApplyEffectEnemyPercentSkill(Targets, EStat.AttackSpeed, null, EStat.AttackSpeed, coefficient);
                break;

            case 4:

                break;
                //skillComponent

        }
    }
}
