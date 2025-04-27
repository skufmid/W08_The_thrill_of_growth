using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using static SynergyManager;
using System.Collections.Generic;


public class CharacterStatusUI : MonoBehaviour
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
    public Slider hpSlider;
    public Slider mpSlider;
    public Image allianceButton;
    public Image classButton;

    public SkillSO skillData; // 캐릭터에 ScriptableObject로 연결된 스킬
    [Header("레벨 표시")]

    [Header("UI 이미지")]
    public Image skillIcon;
    public Sprite defaultSkillIcon;

    Character _character = null;
    private static readonly Color defaultColor = Color.white; // 혹시 못 찾으면 기본색

    private static readonly Dictionary<SynergyType, Color> allianceColors = new Dictionary<SynergyType, Color>
{
    { SynergyType.Kingdom, new Color(1f, 0f, 0f, 0.9f) },
    { SynergyType.Northward, new Color(0f, 0.5f, 1f, 0.9f) },
    { SynergyType.Dark, new Color(0f, 0f, 0f, 0.9f) },
    { SynergyType.HolyLight, new Color(1f, 1f, 0f, 0.9f) },
    // 필요하면 추가
};

    private static readonly Dictionary<CharacterType, Color> classColors = new Dictionary<CharacterType, Color>
{
    { CharacterType.Warrior, new Color(1f, 0.5f, 0f, 0.9f) }, // 주황
    { CharacterType.Tanker, new Color(0.5f, 0f, 1f, 0.9f) },     // 보라
    { CharacterType.Archer, new Color(0f, 1f, 0.5f, 0.9f) },   // 연녹색
    { CharacterType.Wizard, new Color(0f, 1f, 1f, 0.9f) },
    // 필요하면 추가
};
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
        if (character == null || character.isActiveAndEnabled == false)
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
            hpSlider.value = 0;
            mpSlider.value = 0;

            skillIcon.sprite = defaultSkillIcon; // 스킬 아이콘 초기화

            allianceButton.color = defaultColor; // 세력 및 클래스 색상
            classButton.color = defaultColor;
            return;
        }


        damageUIText.text = "데미지";
        attackSpeedUIText.text = "공격 속도";
           
        levelText.text = $"Lv.{character.Level}";
        nameText.text = character.Name;
        hpText.text = $"{character.Hp} / {character.MaxHp}";
        mpText.text = $"{character.Mp} / {character.MaxMp}";
        
        hpSlider.value = (float)character.Hp / character.MaxHp;
        mpSlider.value = (float)character.Mp / character.MaxMp;

        float totalDamage = character.Damage;
        float totalAttackSpeed = character.AttackSpeed;

        // Fixed the problematic line
        damageText.text = $"{totalDamage:F1} (+{character.DefaultDamage:F1} + {character.Damage - character.DefaultDamage:F1})";
        attackSpeedText.text = $"{totalAttackSpeed:F2}(+{character.DefaultAttackSpeed:F1} + {character.AttackSpeed - character.DefaultAttackSpeed:F1})";

        allianceText.text = SynergyManager.SynergyTypeToKorean[character.synergyType];
        classText.text = SynergyManager.CharacterTypeToKorean[character.characterType];

        if (allianceColors.TryGetValue(character.synergyType, out var allianceColor))
        {
            allianceButton.color = allianceColor;
        }
        else
        {
            allianceButton.color = defaultColor;
        }

        if (classColors.TryGetValue(character.characterType, out var classColor))
        {
            classButton.color = classColor;
        }
        else
        {
            classButton.color = defaultColor;
        }

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
