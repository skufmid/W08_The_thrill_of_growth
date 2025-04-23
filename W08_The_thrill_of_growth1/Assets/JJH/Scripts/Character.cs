using UnityEngine;

public class Character:Unit
{
    public bool isUsingSkill;

    public void MeleeAttack()
    {
        Debug.Log("Character MeleeAttack");
    }

    public override void SkillAttack()
    {
        base.SkillAttack();
    }
}
