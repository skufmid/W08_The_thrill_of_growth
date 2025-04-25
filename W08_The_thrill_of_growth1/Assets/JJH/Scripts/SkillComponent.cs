using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class SkillComponent: MonoBehaviour
{
    // 인풋으로 들어가야 하는 것
    // 대상, 데미지, 
    public void DamageSkill(GameObject[] objects, Unit Caster, float amount)
    {
        foreach (GameObject _object in objects)
        {
            var unit = _object.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(amount); // TakeDamage 수정해야함
                Caster.Bloodlust(amount); // 흡혈
            }
        }
    }

    public void ApplySelfDamage(GameObject _object, float ratio)
    {
        var unit = _object.GetComponent<Unit>();
        if (unit != null)
        {
            float value = unit.MaxHp * ratio;
            unit.TakeDamage(value); // TakeDamage 수정해야함
        }
    }

    // 고정된 값 혹은 시전자의 스탯에 따라 효과를 주는 매서드
    public void ApplyEffectAmountSkill(GameObject[] objects, EStat stat, float? duration, float amount) // duration이 null이면 계속 지속되는 효과 
    {
        foreach (var _object in objects)
        {
            var unit = _object.GetComponent<Unit>();
            if (unit != null)
            {
                if (duration == null)
                {
                    ModifyStat(unit, stat, amount);
                }
                else
                {
                    StartCoroutine(CoApplyAndRevert(unit, stat, amount, (float)duration));
                }
            }
        }
    }

    // 시전을 받는 사람의 스탯에 따라 효과를 주는 매서드 // 잃은 체력 비례
    public void ApplyEffectEnemyPercentSkill(GameObject[] objects, EStat stat, float? duration, EStat object_stat, float ratio) // duration이 null이면 계속 지속되는 효과 
    {
        foreach (var _object in objects)
        {
            var unit = _object.GetComponent<Unit>();
            if (unit != null)
            {
                float amount = -1;
                switch (object_stat)
                {
                    case EStat.MaxHp:
                        amount = unit.MaxHp * ratio;
                        break;
                    case EStat.Hp:
                        amount = unit.Hp * ratio;
                        break;
                    case EStat.MaxMp:
                        amount = unit.MaxMp * ratio;
                        break;
                    case EStat.Mp:
                        amount = unit.Mp * ratio;
                        break;
                    case EStat.DefaultAttackSpeed:
                        amount = unit.DefaultAttackSpeed * ratio;
                        break;
                    case EStat.AttackSpeed:
                        amount = unit.AttackSpeed * ratio;
                        break;
                    case EStat.DefaultDamage:
                        amount = unit.DefaultDamage * ratio;
                        break;
                    case EStat.Damage:
                        amount = unit.Damage * ratio;
                        break;

                    case EStat.LostHPRatio: // 잃은 체력 비례
                        amount = (unit.MaxHp - unit.Hp) / unit.MaxHp * ratio;
                        break;
                }


                if (amount < -0.99f)
                {
                    Debug.Log("object_stat가 적절하게 설정되지 않았습니다.");
                }

                if (duration == null)
                {
                    ModifyStat(unit, stat, amount);
                }
                else
                {
                    StartCoroutine(CoApplyAndRevert(unit, stat, amount, (float)duration));
                }
            }
        }
    }

    private IEnumerator CoApplyAndRevert(Unit unit, EStat stat, float amount, float duration)
    {
        // 1) 적용
        ModifyStat(unit, stat, amount);

        // 2) duration 초 대기
        yield return new WaitForSeconds(duration);

        // 3) 원상복구
        ModifyStat(unit, stat, -amount);
    }

    private void ModifyStat(Unit unit, EStat stat, float amount)
    {
        switch (stat)
        {
            case EStat.MaxHp:
                unit.MaxHp += amount;
                break;
            case EStat.Hp:
                unit.Hp += amount;
                break;
            case EStat.MaxMp:
                unit.MaxMp += amount;
                break;
            case EStat.Mp:
                unit.Mp += amount;
                break;
            case EStat.DefaultAttackSpeed:
                unit.DefaultAttackSpeed += amount;
                break;
            case EStat.AttackSpeed:
                unit.AttackSpeed += amount;
                break;
            case EStat.DefaultDamage:
                unit.DefaultDamage += amount;
                break;
            case EStat.Damage:
                unit.Damage += amount;
                break;
        }
    }

    public void RepeatBasicAttack(Character character, GameObject attackTarget, int repeatNum, float ratio)
    {
        StartCoroutine(CoBasicAttack(character, attackTarget, repeatNum, ratio));
    }
    private IEnumerator CoBasicAttack(Character character, GameObject attackTarget, int repeatNum, float ratio)
    {
        float interval = 0.2f;

        for (int i = 0; i < repeatNum; i++)
        {
            yield return new WaitForSeconds(interval);
            if (attackTarget == null) yield break;
        
            Enemy enemy = attackTarget.GetComponent<Enemy>();
            if (enemy == null) yield break;

            character.BasicAttack(); // Enemy 타입으로 전달
            character.LaunchProjectile(0.3f);

            yield return new WaitForSeconds(0.3f); // 투사체 발사 후 대기
            character.DamageEnemy(enemy, ratio);
            
        }
    }


}
