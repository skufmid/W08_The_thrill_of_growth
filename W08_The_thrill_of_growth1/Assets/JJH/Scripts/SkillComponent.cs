using UnityEngine;

public class SkillComponent
{
    // 인풋으로 들어가야 하는 것
    // 대상, 데미지, 
    public void DamageSkill(GameObject[] objects, float damage)
    {
        foreach (GameObject _object in objects)
        {
            var unit = _object.GetComponent<Unit>();
            unit.TakeDamage(); // TakeDamage 수정해야함
        }
    }

    // 대상, 카테고리, 값, 지속시간
    public void CharacterSkill()
    {

    }

    // 대상, 카테고리, 값, 지속시간
    public void DebuffSkill()
    {

    }
}
