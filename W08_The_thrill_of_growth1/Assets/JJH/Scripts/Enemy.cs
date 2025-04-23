using UnityEngine;

public class Enemy:Unit
{
    private void Start()
    {
        base.Init();
        animator = GetComponentInChildren<Animator>();
        Manager.Battle.AddEnemy(gameObject);
        Debug.Log(name);
        beginCombat = true;                             //마나재생 시작하자마자 킬려고
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
    public virtual void DamagePlayer()   //플레이어에게 기본 공격 피해
    {
        
            Character player = attackTarget.GetComponent<Character>();
        player.TakeDamage(Damage);

    }
}
