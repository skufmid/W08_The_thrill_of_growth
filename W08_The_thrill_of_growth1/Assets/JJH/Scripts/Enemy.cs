using UnityEngine;

public class Enemy:Unit
{
    private void Start()
    {
        base.Init();
        animator = GetComponentInChildren<Animator>();
        Manager.Battle.AddEnemy(gameObject);
        Debug.Log(name);
    }
    public override void SkillAttack(float damage)
    {
        base.SkillAttack(Damage);
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

    public override GameObject SelectTarget()
    {
        return BattleManager.Instance.GetTargetByPositionPriority();
        
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log("ouch!" + name);
    }
}
