using UnityEngine;

public class Archer:Character
{
    private BattleManager _battleManager; //전투매니저

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void MeleeAttack()
    {
        base.MeleeAttack(); // 부모 메서드 호출
        Debug.Log("ArcherArrow!"); // 추가 효과
    }
}
