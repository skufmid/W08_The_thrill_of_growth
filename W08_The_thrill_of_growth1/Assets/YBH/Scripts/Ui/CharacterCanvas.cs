using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCanvas : MonoBehaviour
{
    [Header("UI 텍스트")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text damageText;
    public TMP_Text attackSpeedText;
    public TMP_Text allianceText;
    public TMP_Text classText;
    public TMP_Text skillDescText;
    public TMP_Text skillNameText; // 스킬 이름을 표시할 텍스트 필드
    public SkillSO skillData; // 캐릭터에 ScriptableObject로 연결된 스킬
    [Header("레벨 표시")]
    public TMP_Text levelText;

    private void Start()
    {
        //Invoke("Aevent", 1f);
    }
    private void Update()
    {
        //Aevent();
    }
    void Aevent()
    {
        Character character = Manager.Battle.characterList[0].GetComponent<Character>();
        if (character != null)
        {
            SetCharacter(character);
        }
    }
    [Header("UI 이미지")]
    public Image skillIcon;

    public void SetCharacter(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("⚠️ 캐릭터 정보가 null입니다.");
            return;
        }

        nameText.text = character.name;
        hpText.text = $"{character.Hp} / {character.MaxHp}";
        mpText.text = $"{character.Mp} / {character.MaxMp}";

        float totalDamage = character.Damage;
        float totalAttackSpeed = character.AttackSpeed;

        // Fixed the problematic line
        damageText.text = $"{totalDamage:F1} (+{character.DefaultDamage:F1} + {character.Damage - character.DefaultDamage:F1})";
        attackSpeedText.text = $"{totalAttackSpeed:F2}(+{character.DefaultAttackSpeed:F1} + {character.AttackSpeed - character.DefaultAttackSpeed:F1})";

        allianceText.text = character.synergyType.ToString();
        classText.text = character.characterType.ToString();

        // 스킬 정보
        SkillSO skill = System.Array.Find(Manager.Data.Skills, s => s.Id == character.Id);
        if (skill != null)
        {
            skillIcon.sprite = skill.sprite;
            skillDescText.text = skill.SkillDescription;
            skillNameText.text = skill.Name;
            // Replace the problematic line:

            // With the following corrected line:
            skillNameText.text = skill.Name;
        }
        else
        {
            skillIcon.sprite = null;
            skillDescText.text = "스킬 정보 없음";
        }
    }
}
