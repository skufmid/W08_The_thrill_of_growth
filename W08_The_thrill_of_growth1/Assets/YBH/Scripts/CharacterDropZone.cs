using UnityEngine.EventSystems;
using UnityEngine;

public class CharacterDropZone : MonoBehaviour, IDropHandler
{
    public Character character;

    public void OnDrop(PointerEventData eventData)
    {
        Orb orb = eventData.pointerDrag.GetComponent<Orb>();
        if (orb == null) return;

        switch (orb.orbType)
        {
            case OrbType.Damage:
                character.DefaultDamage += orb.value;
                character.Damage += orb.value;
                break;
            case OrbType.AttackSpeed:
                character.DefaultAttackSpeed += orb.value;
                character.AttackSpeed += orb.value;
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
                character.Mp = Mathf.Clamp(character.Mp + mpHeal, 0, character.MaxMp);
                break;
        }

        Destroy(orb.gameObject);
        Debug.Log($"🌟 {character.name} 성장! {orb.orbType} +{orb.value}");
    }
}