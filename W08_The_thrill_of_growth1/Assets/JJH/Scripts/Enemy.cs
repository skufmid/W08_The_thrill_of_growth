using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy:Unit
{
    EnemyInfoUI enemyInfoUI;
    bool _dieOnce;
    public void Awake()
    {
        enemyInfoUI = FindAnyObjectByType<EnemyInfoUI>();
    }
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
        DefaultDamage = 4 + (level - 1) * 4f + Random.Range(-1, 1);
        DefaultAttackSpeed = 0;

        base.Init();
        Debug.Log("Enemy Init");
    }

    public override void SkillAttack(int skillId)
    {
        attackTarget = Manager.Battle.GetTargetByPositionPriority();
        SkillManager.Instance.InvokeEnemySkill(this);
        animator.SetTrigger("Attack");
        LaunchProjectile(0.6f);
    }

    public override void Die()
    {
        base.Die();
        Manager.Battle.RemoveEnemy(gameObject);
        GiveAward();
    }

    public void GiveAward()
    {
        if(_dieOnce) return;
        if (Random.Range(0f, 1f) < 0.1f)
        {
            OrbSpawner.Instance.SpawnRandomOrb(transform.position);
            _dieOnce = true;
        }
        else
        {
            _dieOnce = true;
            //Manager.Game.GetAward...
        }

        //Manager.Game.GetAward...
    }

    private void OnMouseDown()
    {
        enemyInfoUI.SetEnemyUI(this);
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
