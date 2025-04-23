using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class Character:Unit
{
    public int Level = 1;
    public int Star = 1;

    public CombatLine combatLine;
    public CombatLine.linePosition position = CombatLine.linePosition.None;
    public Action basicAttack;
    protected Coroutine _attackRoutine;
    //public SynergyManager.SynergyType synergyType;
    //public SynergyManager.CharacterType characterType;
    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Log(name);
    }
    protected override void Init()
    {
        base.Init();
        Debug.Log("Character Init");
    }
    protected void Start()
    {
        Manager.Battle.AddCharacter(gameObject);
        base.Init();
        Invoke("StartAutoAttack", 1f);

    }

    public void LevelUp()
    {
        if (Level == 30) return;
        Level++;
        if (Level % 10 == 0)
        {
            StarUP();
        }
    }

    public void StarUP()
    {
        Star++;
    }

    public void StartAutoAttack()
    {
        if (_attackRoutine != null)
            StopCoroutine(_attackRoutine);

        _attackRoutine = StartCoroutine(AutoAttackLoop());
    }
    #region 플레이어 기본 공격
    public virtual void BasicAttack()   //기본 공격 모션출력
    {
        animator.SetTrigger("Attack");
        animator.SetFloat("SkillState", 0f);
        Debug.Log("Character MeleeAttack");
    }
    public virtual void DamageEnemy(Enemy Target)   //적에게 기본 공격 피해
    {
        Target.TakeDamage(Damage);
        
    }

    #endregion 플레이어 기본공격
    public override void SkillAttack(int skillId)
    {
        animator.SetTrigger("Attack");
        animator.SetFloat("SkillState", 1.0f);
        base.SkillAttack(skillId);
    }
    public void Onclick()               //플레이어 눌렀을때 하단에 UI패널 나와야되니까.
    {

    }

    protected IEnumerator AutoAttackLoop()//캐릭터 기본 공격 시스템
    {
        float interval = 1f / AttackSpeed;

        while (true)
        {
            yield return new WaitForSeconds(interval);
            attackTarget = Manager.Battle.enemyList[0];
            Debug.Log("AttackLoopStart!"); // 추가 효과
            if (attackTarget != null && !isUsingSkill)
            {
                Enemy enemy = attackTarget.GetComponent<Enemy>(); 
                if (enemy != null)
                {
                    BasicAttack(); // Enemy 타입으로 전달
                    LaunchProjectile();
                    Debug.Log("LaunchProjectile");
                    yield return new WaitForSeconds(0.3f); // 투사체 발사 후 대기
                    DamageEnemy(enemy);
                }
            }
        }
    }
    

}
