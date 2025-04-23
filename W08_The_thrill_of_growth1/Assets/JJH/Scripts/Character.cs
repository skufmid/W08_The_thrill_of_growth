using System;
using UnityEngine;

public class Character:Unit
{
    public bool isUsingSkill;
    public Action basicAttack;
    public CombatLine combatLine;
    public CombatLine.linePosition position = CombatLine.linePosition.None;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Manager.Battle.AddCharacter(gameObject);
        Debug.Log(name);
    }
    public virtual void MeleeAttack()
    {
        animator.SetTrigger("Attack");
        animator.SetFloat("SkillState", 0f);
        Debug.Log("Character MeleeAttack");
    }

    public override void SkillAttack()
    {
        animator.SetTrigger("Attack");
        animator.SetFloat("SkillState", 1.0f);
        base.SkillAttack();
    }
    public void Onclick()               //플레이어 눌렀을때 하단에 UI패널 나와야되니까.
    {

    }



}
