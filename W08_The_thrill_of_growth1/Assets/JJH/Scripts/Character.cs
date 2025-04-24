using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static SynergyManager;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;


public class Character:Unit
{
    public int Level = 1;
    public int Star = 1;

    public CombatLine combatLine;
    public CombatLine.linePosition position = CombatLine.linePosition.None;

    protected Coroutine _attackRoutine;
    public SynergyManager.SynergyType synergyType;
    public SynergyManager.CharacterType characterType;
    public float MaxAttackspeed = 4f; // 최대 공격 속도
    private CharacterCanvas characterCanvas;

    protected virtual void Awake()
    {
        characterCanvas = FindAnyObjectByType<CharacterCanvas>();
    }
    protected void Start()
    {
        Manager.Game.OnEndStage += EndBattle;
        Manager.Game.OnStartStage += StartBattle;

        Init();

        Manager.Battle.AddCharacter(gameObject);
    }
    void OnDisable()
    {
        Manager.Game.OnEndStage -= EndBattle;
        Manager.Game.OnStartStage -= StartBattle;
    }

    protected override void Init()
    {
        CharacterSO character = Array.Find(Manager.Data.Charaters, c => c.Id == Id);

        Name = character.Name;
        DefaultMaxHp = character.BaseHP + (Level - 1) * character.HPPerLevel;
        MaxHp = DefaultMaxHp;
        MaxMp = character.MP;
        DefaultDamage = character.BaseDamage + (Level - 1) * character.DamagePerLevel;
        DefaultAttackSpeed = character.AttackSpeed;
        synergyType = character.SynergyType;
        characterType = character.CharacterType;
        Instantiate(character.Prefabs, transform);
        animator = GetComponentInChildren<Animator>();

        base.Init();
        ProjectileSetting();
        Debug.Log("Character Init");
    }
    void ProjectileSetting()
    {
        if (projectilePrefab != null)
        {
        }
        if (Manager.Data.projectileMap.TryGetValue(characterType, out GameObject prefab))
        {
            projectilePrefab = prefab;
            Debug.Log($"✅ {name}의 Projectile 설정됨: {prefab.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ {characterType} 타입의 Projectile을 찾을 수 없습니다!");
        }
    }
    private void StartBattle()
    {
        MaxHp = DefaultMaxHp;
        Hp = MaxHp;
        Mp = 0;
        AttackSpeed = DefaultAttackSpeed;
        if(DefaultAttackSpeed > MaxAttackspeed)
            DefaultAttackSpeed = MaxAttackspeed;
        Damage = DefaultDamage;
        manaGain = defaultManaGain;

        Invoke("StartAutoAttack", 0.5f);
    }

    private void EndBattle()
    {
        Debug.Log("FinishBattle 실행");
    }

    public void LevelUp()
    {
        if (Level == 30) return;
        Level++;
        if (Level % 10 == 0)
        {
            StarUP();
        }
        CharacterSO character = Array.Find(Manager.Data.Charaters, c => c.Id == Id);
        if (character == null) return;

        DefaultMaxHp = character.BaseHP + (Level - 1) * character.HPPerLevel;
        DefaultDamage = character.BaseDamage + (Level - 1) * character.DamagePerLevel;

    }

    public void StarUP()
    {
        Star++;
    }
    public override void Die()
    {
        base.Die();
        Manager.Battle.RemoveCharacter(gameObject);
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
        if (!Manager.Battle.isInBattle) return;

        animator.SetTrigger("Attack");
        animator.SetFloat("SkillState", 0f);
    }
    public virtual void DamageEnemy(Enemy Target, float ratio=1f)   //적에게 기본 공격 피해
    {
        Target.TakeDamage(Damage * ratio);
        Manager.UI.floatingTextManager.ShowDamage(Damage, Target.transform.position);

    }

    #endregion 플레이어 기본공격
    public override void SkillAttack(int skillId)
    {
        if (!Manager.Battle.isInBattle) return;
        animator.SetTrigger("Attack");
        animator.SetFloat("SkillState", 1.0f);
        base.SkillAttack(skillId);
    }

    private void OnMouseDown()
    {
        characterCanvas?.SetCharacterUI(this);
    }

    protected IEnumerator AutoAttackLoop()//캐릭터 기본 공격 시스템
    {
        float interval = 1f / AttackSpeed;

        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (!Manager.Battle.isInBattle) yield break;

            attackTarget = Manager.Battle.enemyList[0];
            if (attackTarget != null)
            {
                Enemy enemy = attackTarget.GetComponent<Enemy>(); 
                if (enemy != null)
                {
                    BasicAttack(); // Enemy 타입으로 전달
                    LaunchProjectile();
                    yield return new WaitForSeconds(0.6f); // 투사체 발사 후 대기
                    DamageEnemy(enemy);
                }
            }
        }
    }




}
