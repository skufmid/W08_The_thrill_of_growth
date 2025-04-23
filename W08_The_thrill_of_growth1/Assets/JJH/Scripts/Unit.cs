using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public abstract class Unit : MonoBehaviour
{
    //기본스텟
    public float MaxHp;             //최대체력
    public float Hp;                //현재체력
    public float MaxMp;             //최대마나
    public float Mp;                //현재마나
    public float DefaultAttackSpeed;//기본공격속도
    public float AttackSpeed;       //공격속도
    public float DefaultDamage;     //기본공격력
    public float Damage;            //공격력
    public float Vampiric;          //흡혈수치
    //애니메이션
    public Animator animator;
    public bool isUsingSkill;

    //전투관련
    public GameObject attackTarget;         //적 타겟
    public GameObject projectilePrefab;     //투사체 프리팹

    //내부 상태
    protected bool isAttacking = false;
    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        Hp = MaxHp;
        Mp = 0;
        AttackSpeed = DefaultAttackSpeed;
        Damage = DefaultDamage;
    }

    public virtual void SkillAttack(float damage)
    {
        Debug.Log("Unit SkillAttack");
    }

    public virtual void TakeDamage(float damage)
    {
        Debug.Log("Unit TakeDamage");
        Hp -= damage;
        if (Hp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log("Unit Die");
    }

    public virtual GameObject SelectTarget()
    {
        Debug.Log("Unit SelectTarget");
        return null;
    }
    public virtual void LaunchProjectile()//투사체 발사(공격에 붙히는 용도 이벤트)
    {
        if (projectilePrefab == null || attackTarget == null) return;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        StartCoroutine(MoveProjectile(projectile, attackTarget.transform));
        
    }
    private IEnumerator MoveProjectile(GameObject proj, Transform target) // 투사체 이동
    {
        Vector3 startPos = proj.transform.position;
        Vector3 endPos = target.position;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (proj == null)  yield break; 

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            proj.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        proj.transform.position = endPos;
        Destroy(proj);
    }

}
