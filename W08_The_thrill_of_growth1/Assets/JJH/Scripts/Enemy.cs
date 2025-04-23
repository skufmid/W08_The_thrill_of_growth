using UnityEngine;

public class Enemy:Unit
{
    public override void SkillAttack()
    {
        base.SkillAttack();
    }

    public override void Die()
    {
        base.Die();
        GiveAward();
    }

    public void GiveAward()
    {
        //Manager.Game.GetAward...
    }
}
