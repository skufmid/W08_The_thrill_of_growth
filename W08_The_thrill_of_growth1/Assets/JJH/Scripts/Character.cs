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
    [SerializeField] private GameObject statusUIPrefab;  // Inspector에서 할당할 UI 프리팹
    private UnitStatusUI statusUI;  // 캐릭터의 상태 UI
    bool hasPassive = false;

    protected virtual void Awake()
    {
        characterCanvas = FindAnyObjectByType<CharacterCanvas>();
    }
    protected void Start()
    {
        Manager.Game.OnEndStage += EndBattle;
        Manager.Game.OnStartStage += StartBattle;

        Init();
        if(Id == 7)
        {
            hasPassive = true;
            Debug.Log("Passive Skill");
        }
        Manager.Battle.AddCharacter(gameObject);

        // 상태 UI 생성
        if (statusUIPrefab != null)
        {
            GameObject uiObj = Instantiate(statusUIPrefab, GameObject.Find("UICanvas").transform);
            statusUI = uiObj.GetComponent<UnitStatusUI>();
            if (statusUI != null)
            {
                statusUI.SetTarget(this);
            }
        }
    }
    void OnDisable()
    {
        Manager.Game.OnEndStage -= EndBattle;
        Manager.Game.OnStartStage -= StartBattle;

        // UI 제거
        if (statusUI != null)
        {
            Destroy(statusUI.gameObject);
        }
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
        // UI 제거
        if (statusUI != null)
        {
            Destroy(statusUI.gameObject);
        }
        base.Die();
        Manager.Battle.RemoveCharacter(gameObject);
    }

    public void StartAutoAttack()
    {
        if (_attackRoutine != null) StopCoroutine(_attackRoutine);
            { 
                _attackRoutine = StartCoroutine(AutoAttackLoop());
            }
    }
    #region 플레이어 기본 공격
    public virtual void BasicAttack()   //기본 공격 모션출력
    {
        if (!Manager.Battle.isInBattle) return;

        animator.SetTrigger("Attack");
        switch (characterType)
        {
            case SynergyManager.CharacterType.Wizard:
                animator.SetFloat("NormalState", 1f);
                break;
            case SynergyManager.CharacterType.Archer:
                animator.SetFloat("NormalState", 0.5f);
                break;
            default:
                animator.SetFloat("NormalState", 0f);
                break;
        }
        animator.SetFloat("SkillState", 0f);
    }
    public virtual void DamageEnemy(Enemy Target, float ratio=1f)   //적에게 기본 공격 피해
    {
        Target.TakeDamage(Damage * ratio);
    }

    #endregion 플레이어 기본공격
    public override void SkillAttack(int skillId)
    {
        if (hasPassive) return;
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
        while (true)
        {
            float interval = 1f / AttackSpeed;
            float basespeed = 0.5f;             //기본 투사체 속도
            float projectileSpeed = basespeed / AttackSpeed;
 
            yield return new WaitForSeconds(interval);
            
            if (!Manager.Battle.isInBattle) yield break;

            if (Manager.Battle.enemyList.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, Manager.Battle.enemyList.Count);
                attackTarget = Manager.Battle.enemyList[randomIndex];
            }
            if (attackTarget != null)
            {

                Enemy enemy = attackTarget.GetComponent<Enemy>(); 
                if (enemy != null)
                {
                    BasicAttack(); // Enemy 타입으로 전달
                    LaunchProjectile(projectileSpeed);
                    yield return new WaitForSeconds(projectileSpeed); // 투사체 발사 후 대기
                    DamageEnemy(enemy);
                }
            }
            // 패시브: 야성 연사
            if (hasPassive && characterType == SynergyManager.CharacterType.Archer)
            {
                float chance = 0.2f + (Mathf.Max(Star - 1, 0) * 0.05f);

                // To resolve the ambiguity between 'UnityEngine.Random' and 'System.Random', explicitly specify the namespace for 'Random' usage.

                if (UnityEngine.Random.Range(0f, 1f) <= 1.01f)
                {
                    Enemy enemy = attackTarget.GetComponent<Enemy>();

                    BasicAttack();
                    LaunchProjectile(0.2f);
                    yield return new WaitForSeconds(0.2f);
                    DamageEnemy(enemy);
                }
                if (UnityEngine.Random.Range(0f, 1f) <= chance)
                {
                    Enemy enemy = attackTarget.GetComponent<Enemy>();

                    BasicAttack();
                    LaunchProjectile(0.2f);
                    yield return new WaitForSeconds(0.2f);
                    DamageEnemy(enemy);
                }
            }
            if(synergyType == SynergyManager.SynergyType.Northward)
            {
                float count = Manager.Synergy.northWard;
                float chance = 0f;

                if (count >= 4) chance = 0.2f;
                else if (count == 3) chance = 0.1f;
                else if (count == 2) chance = 0.05f;

                if (UnityEngine.Random.Range(0f, 1f) <= chance)
                {
                    Enemy enemy = attackTarget.GetComponent<Enemy>();

                    BasicAttack();
                    LaunchProjectile(0.2f);
                    yield return new WaitForSeconds(0.2f);
                    DamageEnemy(enemy);
                }
                if (UnityEngine.Random.Range(0f, 1f) <= chance)
                {
                    Enemy enemy = attackTarget.GetComponent<Enemy>();

                    BasicAttack();
                    LaunchProjectile(0.2f);
                    yield return new WaitForSeconds(0.2f);
                    DamageEnemy(enemy);
                }
            }
        }
    }
}
