using UnityEngine;
using UnityEngine.TextCore.Text;

public class Enemy:Unit
{
    EnemyStatusUI enemyInfoUI;
    bool _dieOnce;
    [SerializeField] private GameObject hpBarPrefab;  // HP바 프리팹
    private EnemyHPBar enemyHPBar;

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
    }

    protected override void Init()
    {
        int stage = Manager.Game.stageNum;
        Debug.Log($"{stage} level 해골병사 소환");
        Name = "해골 병사";

        // 스테이지별 총합 기준, 적 마리수(7)로 나눔
        float totalHp = 675f * stage;
        float totalDamage = 18f * stage;
        int enemyCount = 7; // Grid에서 실제 생성되는 적 마리수와 맞춤

        float unitHp = totalHp / enemyCount;
        float unitDamage = totalDamage / enemyCount;

        DefaultMaxHp = unitHp + Random.Range(-5f, 5f);
        MaxHp = DefaultMaxHp;
        MaxMp = Random.Range(30, 70);
        DefaultDamage = unitDamage + Random.Range(-1f, 1f);
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
        if (enemyHPBar != null)
        {
            Destroy(enemyHPBar.gameObject);
        }
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
