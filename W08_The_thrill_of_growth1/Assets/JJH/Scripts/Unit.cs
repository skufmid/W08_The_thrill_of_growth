using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public float MaxHp;
    public float Hp;
    public float MaxMp;
    public float Mp;
    public float DefaultAttackSpeed;
    public float AttackSpeed;
    public float DefaultDamage;
    public float Damage;

    private Animator animator;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Hp = MaxHp;
        Mp = 0;
        AttackSpeed = DefaultAttackSpeed;
        Damage = DefaultDamage;
    }

    public virtual void SkillAttack()
    {
        Debug.Log("Unit SkillAttack");
    }

    public virtual void TakeDamage()
    {
        Debug.Log("Unit TakeDamage");
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
}
