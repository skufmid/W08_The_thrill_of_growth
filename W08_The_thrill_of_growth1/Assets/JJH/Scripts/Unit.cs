using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public abstract class Unit : MonoBehaviour
{
    public int Id;                  // ID  0~15: 캐릭터, 100~: 적
    //기본스텟
    public string Name;             //이름
    public float DefaultMaxHp;     //기본최대체력
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
    //전투관련
    public GameObject attackTarget;         //적 타겟
    public GameObject projectilePrefab;     //투사체 프리팹
    public bool beginCombat = true;                //전투시작관련
    public float defaultManaGain = 10f;     //공격중인지
    public float manaGain;                  //마나 회복량
    List<GameObject> projectileList = new List<GameObject>(); //타겟 리스트
    //내부 상태
    protected bool isAttacking = false;
    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }
    private void Start()
    {
        
    }

    protected virtual void Init()
    {
        Hp = MaxHp;
        Mp = 0;
        AttackSpeed = DefaultAttackSpeed;
        Damage = DefaultDamage;
        manaGain = defaultManaGain;

        StartCoroutine(CoManaGain(manaGain));
    }

    public virtual void SkillAttack(int skillId)
    {
       SkillManager.Instance.InvokeSkill(this, skillId);
    }

    public virtual void TakeDamage(float damage)
    {
        Hp -= damage;
        Manager.UI.floatingTextManager.ShowDamage(damage, transform.position);
        if (Hp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log($"Unit {Name} Die");
        gameObject.SetActive(false); 
    }

    public virtual GameObject SelectTarget()
    {
        return null;
    }
    public virtual void LaunchProjectile(float duration)//투사체 발사(공격에 붙히는 용도 이벤트)
    {
        if (projectilePrefab == null || attackTarget == null) return;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        StartCoroutine(MoveProjectile(projectile, attackTarget.transform, duration));
        Debug.Log("Enemy?");
        
    }
    private IEnumerator MoveProjectile(GameObject proj, Transform target, float duration) // 투사체 이동
    {
        Vector3 startPos = proj.transform.position;
        Vector3 endPos = target.position;
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

    public virtual IEnumerator CoManaGain(float Manas) // 마나 회복
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (Manager.Battle.isInBattle == false) continue;
            Mp += manaGain;

            if(Mp >= MaxMp)
            {
                SkillAttack(Id < 100 ? Id : 100); // 캐릭터는 똑같은 스킬 ID 실행, 적은 스킬 100 실행
                Mp = Mp - MaxMp;
            }
        }
    }
    public void Bloodlust(float Damage)
    {
        float amount = Damage * Vampiric;
        Hp += amount;
        if(Hp > MaxHp)Hp = MaxHp;
    }
}
