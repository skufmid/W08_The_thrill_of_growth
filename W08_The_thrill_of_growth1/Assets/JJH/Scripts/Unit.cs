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

    public virtual void MeleeAttack()
    {
        Debug.Log("Unit MeleeAttack");
    }
    
    public virtual void SkillAttack()
    {
        Debug.Log("Unit SkillAttack");
    }

    public virtual void TakeDamage()
    {
        Debug.Log("Unit TakeDamage");
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
