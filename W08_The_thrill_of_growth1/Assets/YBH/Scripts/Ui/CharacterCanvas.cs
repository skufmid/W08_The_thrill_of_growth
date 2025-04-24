using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CharacterCanvas : MonoBehaviour
{
    [Header("UI 텍스트")]
    public TMP_Text levelText;
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text damageText;
    public TMP_Text damageUIText;
    public TMP_Text attackSpeedText;
    public TMP_Text attackSpeedUIText;
    public TMP_Text allianceText;
    public TMP_Text classText;
    public TMP_Text skillDescText;
    public TMP_Text skillNameText; // 스킬 이름을 표시할 텍스트 필드
    public SkillSO skillData; // 캐릭터에 ScriptableObject로 연결된 스킬
    [Header("레벨 표시")]

    [Header("UI 이미지")]
    public Image skillIcon;
    public Sprite defaultSkillIcon;

    Character _character = null;
    private void Start()
    {
        StartCoroutine(CoSetCharacterUI());
    }


    IEnumerator CoSetCharacterUI()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            SetCharacterUI(_character);
        }
    }
    public void SetCharacterUI(Character character)
    {
        _character = character;
        if (character == null)
        {
            levelText.text = "";
            nameText.text = "";
            hpText.text = "";
            mpText.text = "";
            damageText.text = "";
            damageUIText.text = "";
            attackSpeedText.text = "";
            attackSpeedUIText.text = "";
            allianceText.text = "";
            classText.text = "";
            skillDescText.text = "";
            skillNameText.text = "";

            skillIcon.sprite = defaultSkillIcon; // 스킬 아이콘 초기화
            return;
        }


        damageUIText.text = "데미지";
        attackSpeedUIText.text = "공격 속도";
           
        levelText.text = $"Lv.{character.Level}";
        nameText.text = character.Name;
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
