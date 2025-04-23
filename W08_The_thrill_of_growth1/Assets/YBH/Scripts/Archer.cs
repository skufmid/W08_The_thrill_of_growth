using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Archer:Character
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject archerArrowPrefab;
    public List<Sprite> archerSpumlist;
    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
        projectilePrefab = archerArrowPrefab;
        beginCombat = true;
    }

    public override void BasicAttack()
    {
        base.BasicAttack(); // 부모의 기본 공격 실행
        Debug.Log("ArcherArrow!"); // 아쳐 전용 효과 추가
    }
}
