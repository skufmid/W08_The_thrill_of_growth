using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy : Unit
{
    EnemyStatusUI enemyInfoUI;
    bool _dieOnce;
    [SerializeField] private GameObject hpBarPrefab;  // HP바 프리팹
    private EnemyHPBar enemyHPBar;
    [SerializeField] bool isBoss = false; // 보스 여부
    ParticleSystem deathEffect; // 죽을 때 나오는 이펙트
    public void Awake()
    {
        enemyInfoUI = FindAnyObjectByType<EnemyStatusUI>();
        // HP바 생성 (UICanvas의 자식으로)
        if (hpBarPrefab != null)
        {
            Canvas uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
            if (uiCanvas != null)
            {
                GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
                enemyHPBar = hpBar.GetComponent<EnemyHPBar>();
                enemyHPBar.GetComponent<EnemyHPBar>().enemy = this;
            }
        }
    }

    private void Start()
    {
        Init();
        animator = GetComponentInChildren<Animator>();
        Manager.Battle.AddEnemy(gameObject);
        Debug.Log($"{name} Init");
        beginCombat = true;                             //마나재생 시작하자마자 킬려고
        if(isBoss)
        {
            deathEffect.Play();
        }
    }

    protected override void Init()
    {
        int enemyCount = 7;

        if (isBoss)
        {
            deathEffect= GetComponentInChildren<ParticleSystem>();
            enemyCount = 1;
        }
            int stage = Manager.Game.stageNum;
            Debug.Log($"{stage} level 해골병사 소환");
            Name = "해골 병사";

            // 스테이지별 총합 기준, 적 마리수(7)로 나눔
            float totalHp = 700f * stage;
            float totalDamage = 22f * stage + 56;
            float unitHp = totalHp / enemyCount;
            float unitDamage = totalDamage / enemyCount;
            DefaultMaxHp = unitHp + Random.Range(-5f, 5f);
            MaxHp = DefaultMaxHp;
            MaxMp = Random.Range(30, 70);
            DefaultDamage = unitDamage + Random.Range(-1f, 1f);
            DefaultAttackSpeed = 0;
        if(isBoss)
        {
            unitHp *= totalHp * 30;
            MaxMp = 30;
        }
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
        if (enemyHPBar != null)
        {
            Destroy(enemyHPBar.gameObject);
        }
        if(isBoss)
        {
            if (deathEffect != null)
            {
                deathEffect.Play();
            }
        }
        Manager.Battle.RemoveEnemy(gameObject);
        GiveAward();
    }

    public void GiveAward()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (_dieOnce) return;

        float potionChance = 0.06f;
        float itemChance = 0.06f;
        float uniqueChance = 0.03f;

        if (isBoss)
        {
            // 보스는 드랍률 강화
            potionChance = 0.1f;
            itemChance = 1f;
            uniqueChance = 0.05f;
        }

        OrbSpawner.Instance.SpawnOrbsOnDeath(screenPosition, potionChance, itemChance, uniqueChance);

        Destroy(gameObject);
        _dieOnce = true;
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
    public void bossCheck(int bossNumber) // 스테이지에 따라 보스 외형을 다르게
    {
        // 보스 상태 이름 배열
        string[] bossStates = { "Boss_Conquest", "Boss_Famine", "Boss_War", "Boss_Death" };
        foreach (string stateName in bossStates)
        {
            Transform stateTransform = transform.Find(stateName);
            if (stateTransform != null)
            {
                stateTransform.gameObject.SetActive(false);
            }
        }

        // 현재 보스 번호에 해당하는 상태 활성화
        if (bossNumber >= 0 && bossNumber < bossStates.Length)
        {
            Transform targetState = transform.Find(bossStates[bossNumber]);
            if (targetState != null)
            {
                targetState.gameObject.SetActive(true);
                Debug.Log($"보스 상태 활성화: {bossStates[bossNumber]}");
            }
            else
            {
                Debug.LogWarning($"❗ {bossStates[bossNumber]} 상태를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"❗ 유효하지 않은 보스 번호: {bossNumber}");
        }
    }
}
