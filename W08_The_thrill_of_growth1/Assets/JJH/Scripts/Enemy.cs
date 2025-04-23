using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy:Unit
{
    private void Start()
    {
        Init();
        animator = GetComponentInChildren<Animator>();
        Manager.Battle.AddEnemy(gameObject);
        Debug.Log($"{name} Init");
        beginCombat = true;                             //마나재생 시작하자마자 킬려고
    }

    protected override void Init()
    {
        int level = Manager.Game.stageNum;
        Debug.Log($"{level} level 해골병사 소환");
        Name = "해골 병사";
        DefaultMaxHp = 30 + (level - 1) * 5f + Random.Range(-5, 5);
        MaxHp = DefaultMaxHp;
        MaxMp = Random.Range(25, 65);
        DefaultDamage = 5 + (level - 1) * 3f + Random.Range(-2, 2);
        DefaultAttackSpeed = 0;

        base.Init();
        Debug.Log("Enemy Init");
    }

    public override void SkillAttack(int skillId)
    {
        SkillManager.Instance.InvokeEnemySkill(this);
    }

    public override void Die()
    {
        base.Die();
        Manager.Battle.RemoveEnemy(gameObject);
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
    }
    public virtual void DamagePlayer()   //플레이어에게 기본 공격 피해
    {
        
        Character player = attackTarget.GetComponent<Character>();
        player.TakeDamage(Damage);

    }
}
