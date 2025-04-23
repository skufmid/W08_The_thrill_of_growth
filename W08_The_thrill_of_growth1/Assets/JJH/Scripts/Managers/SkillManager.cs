using System;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    SkillComponent skillComponent;
    public void Init()
    {
        //skillComponent = GetComponent<SkillComponent>();
    }

    public void InvokeSkill(int skillId)
    {
        SkillSO skill = Array.Find(Manager.Data.Skills, s => s.Id == skillId);

        //switch (skillId)
        //{
        //    case 0:
        //        _objects = skillComponent.ApplyEffectEnemyPercentSkill()
        //        break;
        //}
    }
}
