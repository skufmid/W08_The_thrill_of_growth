using UnityEngine;

public class Archer:Character
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject archerArrowPrefab;

    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
        projectilePrefab = archerArrowPrefab;

    }

    void Update()
    {
        
    }
    public override void BasicAttack()
    {
        base.BasicAttack(); // 부모의 기본 공격 실행
        Debug.Log("ArcherArrow!"); // 아쳐 전용 효과 추가
    }
}
