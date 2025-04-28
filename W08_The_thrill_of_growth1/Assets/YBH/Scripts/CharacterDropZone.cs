using UnityEngine.EventSystems;
using UnityEngine;

public class CharacterDropZone : MonoBehaviour, IDropHandler
{
    public Character character;

    public void OnDrop(PointerEventData eventData)
    {
        Orb orb = eventData.pointerDrag.GetComponent<Orb>();
        if (orb == null) return;
        character.OrbEffectActive(orb.orbEffectColor);
        switch (orb.orbType)
        {
            case OrbType.Damage:
                character.DefaultDamage += orb.value;
                character.Damage += orb.value;
                break;
            case OrbType.AttackSpeed:
                if (character.DefaultAttackSpeed < character.MaxAttackspeed)
                {
                    character.DefaultAttackSpeed += orb.value;
                    character.AttackSpeed += orb.value;
                }
                else
                {
                    character.DefaultAttackSpeed = character.MaxAttackspeed;
                    character.AttackSpeed = character.MaxAttackspeed;
                }
                break;
            case OrbType.MaxHP:
                character.DefaultMaxHp += orb.value;
                character.MaxHp += orb.value;
                character.Hp += orb.value;
                break;
            case OrbType.ManaGain:
                character.defaultManaGain += orb.value;
                character.manaGain += orb.value;
                break;

            case OrbType.Potion:
                float hpHeal = character.MaxHp * (orb.value / 100f); // 💊 퍼센트 회복  
                character.Hp = Mathf.Clamp(character.Hp + hpHeal, 0, character.MaxHp);
                break;

            case OrbType.ManaPotion:
                float mpHeal = character.MaxMp * (orb.value / 100f); // 🔵 퍼센트 회복  
                character.Mp += mpHeal;
                break;
            case OrbType.Vampiric:
                character.DefaultVampiric += orb.value; 
                character.Vampiric = character.DefaultVampiric; 
                break;
        }

        Destroy(orb.gameObject); // 혹시 부모가 없으면 그냥 자신만 삭제  

        Debug.Log($"🌟 {character.name} 성장! {orb.orbType} +{orb.value}");
    }
}
