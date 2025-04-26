using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public class SynergyManager
{
    public enum SynergyType { Kingdom, Northward, Dark, HolyLight }
    public enum CharacterType { Tanker, Warrior, Wizard, Archer }

    public static readonly Dictionary<SynergyType, string> SynergyTypeToKorean = new Dictionary<SynergyType, string>
{
    { SynergyType.Kingdom, "왕국" },
    { SynergyType.Northward, "북방" },
    { SynergyType.Dark, "어둠" },
    { SynergyType.HolyLight, "빛" }
};

public static readonly Dictionary<CharacterType, string> CharacterTypeToKorean = new Dictionary<CharacterType, string>
{
    { CharacterType.Tanker, "탱커" },
    { CharacterType.Warrior, "전사" },
    { CharacterType.Wizard, "마법사" },
    { CharacterType.Archer, "궁수" }
};

    Canvas _synergyCanvas;
    Image[] _synergySlots; // 슬롯 Image 배열
    int _currentIndex = 0; // 다음 사용될 슬롯 위치
    HashSet<Sprite> _usedIcons = new(); // 중복 방지
    Text _synergyText; // 시너지 텍스트
    public float northWard; // 북부시너지 카운트
    public void Init()
    {
        Manager.Game.OnLateStartStage += () => SynergyUpdate(); // 스테이지 시작 시 시너지 평가
        ClearSynergyUI();
    }
    public void CanvasInit()
    {
        _synergyCanvas = GameObject.Find("SynergyCanvas").GetComponent<Canvas>();
        Transform panel = _synergyCanvas.transform.Find("SynergyPanel");
        _synergySlots = panel.GetComponentsInChildren<Image>(); // 슬롯 미리 연결
        _currentIndex = 0;
        _usedIcons.Clear();

        foreach (var slot in _synergySlots)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0); // 알파 0으로 안 보이게
        }
    }

    public void EvaluateSynergies(List<GameObject> characterList)
    {
        //시너지카운트
        List<SynergyType> activeFactions = new List<SynergyType>();
        List<int> activeFactionCounts = new List<int>();
        //캐릭터카운트
        List<CharacterType> activeClasses = new List<CharacterType>();
        List<int> activeClassCounts = new List<int>();

        Dictionary<SynergyType, int> synergyCounts = new Dictionary<SynergyType, int>();
        Dictionary<CharacterType, int> typeCounts = new Dictionary<CharacterType, int>();
        List<CharacterType> activatedTypes = new();     // 직업 시너지 발동 목록
        List<SynergyType> activatedFactions = new();    // 세력 시너지 발동 목록

        foreach (GameObject characterObj in characterList)
        {
            Character character = characterObj.GetComponent<Character>();
            if (character == null) continue;

            // 시너지 타입 카운트
            if (!synergyCounts.ContainsKey(character.synergyType))
                synergyCounts[character.synergyType] = 0;
            synergyCounts[character.synergyType]++;

            // 캐릭터 타입 카운트
            if (!typeCounts.ContainsKey(character.characterType))
                typeCounts[character.characterType] = 0;
            typeCounts[character.characterType]++;

        }
        // (2) 조건에 맞으면 활성화 리스트에 추가
        foreach (var pair in synergyCounts)
        {
            if (pair.Key == SynergyType.Dark)
            {
                if (pair.Value >= 4)
                {
                    activeFactions.Add(pair.Key);
                    activeFactionCounts.Add(pair.Value);
                }
            }
            else
            {
                if (pair.Value >= 2)
                {
                    activeFactions.Add(pair.Key);
                    activeFactionCounts.Add(pair.Value);
                }
            }
        }
        foreach (var pair in typeCounts)
        {
            if (pair.Value >= 2)
            {
                activeClasses.Add(pair.Key);
                activeClassCounts.Add(pair.Value);
            }
        }

        //-------- 직업 시너지 ------------------
        // 탱커 시너지
        if (typeCounts.TryGetValue(CharacterType.Tanker, out int tankCount))
        {
            if (tankCount >= 4)
            {
                ApplyHpBuff(0.40f);
                activatedTypes.Add(CharacterType.Tanker);
            }
            else if (tankCount >= 3)
            {
                ApplyHpBuff(0.20f);
                activatedTypes.Add(CharacterType.Tanker);
            }
            else if (tankCount >= 2)
            {
                ApplyHpBuff(0.10f);
                activatedTypes.Add(CharacterType.Tanker);
            }
        }
        // 전사 시너지
        if (typeCounts.TryGetValue(CharacterType.Warrior, out int warriorCount))
        {
            if (warriorCount >= 4)
            {
                ApplyVampiricBuff(0.2f);
                activatedTypes.Add(CharacterType.Warrior);
            }
            else if (warriorCount >= 3)
            {
                activatedTypes.Add(CharacterType.Warrior);
                ApplyVampiricBuff(0.1f);
            }
            else if (warriorCount >= 2)
            {
                activatedTypes.Add(CharacterType.Warrior);
                ApplyVampiricBuff(0.05f);
            }
        }
        // 마법사 시너지
        if (typeCounts.TryGetValue(CharacterType.Wizard, out int wizardCount))
        {
            if (wizardCount >= 4)
            {
                activatedTypes.Add(CharacterType.Wizard);
                ApplyManaBuff(4f);
            }
            else if (wizardCount >= 3)
            {
                activatedTypes.Add(CharacterType.Wizard);
                ApplyManaBuff(2f);
            }
            else if (wizardCount >= 2)
            {
                activatedTypes.Add(CharacterType.Wizard);
                ApplyManaBuff(1f);
            }
        }
        // 궁수 시너지
        if (typeCounts.TryGetValue(CharacterType.Archer, out int archerCount))
        {
            if (archerCount >= 4)
            {
                activatedTypes.Add(CharacterType.Archer);
                ApplyAttackSpeedBuff(0.40f);
            }
            else if (archerCount >= 3)
            {
                activatedTypes.Add(CharacterType.Archer);
                ApplyAttackSpeedBuff(0.20f);
            }
            else if (archerCount >= 2)
            {
                activatedTypes.Add(CharacterType.Archer);
                ApplyAttackSpeedBuff(0.10f);
            }
            
        }


        //-------- 세력 시너지 ------------------
        // 왕국 연합
        if (synergyCounts.TryGetValue(SynergyType.Kingdom, out int kingdomCount))
        {
            if (kingdomCount >= 4)
            {
                activatedFactions.Add(SynergyType.Kingdom);
                ApplyAllStatsBuff(0.30f);
            }
            else if (kingdomCount >= 3)
                {
                activatedFactions.Add(SynergyType.Kingdom);
                ApplyAllStatsBuff(0.20f);
            }
            else if (kingdomCount >= 2)
            {
                activatedFactions.Add(SynergyType.Kingdom);
                ApplyAllStatsBuff(0.10f);
            }
        }
        //북방 부족


        if (synergyCounts.TryGetValue(SynergyType.Northward, out int northCount)) // 공격 1회 추가공격으로 수정해야함.
        {
            if (northCount >= 4)
            {
                northWard = 4f;
                activatedFactions.Add(SynergyType.Northward);
                ApplyAttackBuff(0.40f);
            }
            else if (northCount >= 3)
            {
                northWard = 3f;
                activatedFactions.Add(SynergyType.Northward);
                ApplyAttackBuff(0.20f);
            }
            else if (northCount >= 2)
            {
                northWard = 2f;
                activatedFactions.Add(SynergyType.Northward);
                ApplyAttackBuff(0.10f);
            }
        }
        //어둠의 교단
        if (synergyCounts.TryGetValue(SynergyType.Dark, out int darkCount))
        {
            if (darkCount >= 4)
            {
                activatedFactions.Add(SynergyType.Dark);
                ApplyAttackBuff(0.50f);
            }
        }
        //성광 교단
        if (synergyCounts.TryGetValue(SynergyType.HolyLight, out int holyCount))
        {
            if (holyCount >= 4)
            {
                activatedFactions.Add(SynergyType.HolyLight);
                SynergyHelper.Instance.RunCoroutine(0.2f);
            }
            else if (holyCount >= 3)
            {
                activatedFactions.Add(SynergyType.HolyLight);
                SynergyHelper.Instance.RunCoroutine(0.1f);
            }
            else if (holyCount >= 2)
            {
                activatedFactions.Add(SynergyType.HolyLight);
                SynergyHelper.Instance.RunCoroutine(0.05f);
            }
        }

        DisplayActiveSynergies(activeClasses, activeClassCounts, activeFactions, activeFactionCounts);
    }


    void DisplayActiveSynergies(List<CharacterType> activeTypes, List<int> activeClassCounts, List<SynergyType> activeFactions, List<int> activeFactionCounts)// 팩션 Ui
    {
        for (int i = 0; i < activeTypes.Count; i++)
        {
            CharacterType type = activeTypes[i];
            int count = activeClassCounts[i];
            Sprite icon = Manager.Data.GetCharacterIcon(activeTypes[i]);
            string desc = Manager.Data.characterDataList
            .First(data => data.charactertType == activeTypes[i])
            .description;

            UpdateUI(icon, desc, count, 4f); // 캐릭터 아이콘 표시
        }

        for (int i = 0; i < activeFactions.Count; i++)
        {
            SynergyType type = activeFactions[i];
            int count = activeFactionCounts[i];
            Sprite icon = Manager.Data.GetSynergyIcon(activeFactions[i]);
            string desc = Manager.Data.synergyDataList
            .First(data => data.synergyType == activeFactions[i])
            .description;
            int maxRequired = 4; // 최대 4명 기준

            UpdateUI(icon, desc, count, maxRequired); // 세력 아이콘 표시
        }
    }
    void UpdateUI(Sprite icon, string description, float count, float maxCount) // UI 업데이트
    {
        if (_synergySlots == null)
        {
            Debug.LogError("❗ _synergySlots 배열이 null입니다.");
            return;
        }
        if (_usedIcons.Contains(icon)) return;               // 중복 아이콘 표시 방지
        if (_currentIndex >= _synergySlots.Length) return;   // 슬롯 넘치면 무시

        Image slot = _synergySlots[_currentIndex];

        slot.sprite = icon;
        slot.color = Color.white;                            // 표시되도록 설정
        slot.preserveAspect = true;
        Image iconImage = slot.GetComponent<Image>();
        if (iconImage != null)
            iconImage.sprite = icon;
        TMP_Text countText = slot.GetComponentInChildren<TMP_Text>();
        if (countText != null)
            countText.text = $"{count}/{maxCount}";
        if (iconImage != null)
        {
            countText.color = GetSynergyColor((int)count, (int)maxCount);
        }
        SynergySlotHover hover = slot.GetComponent<SynergySlotHover>();
        if (hover != null)
            hover.description = description;

        slot.gameObject.SetActive(true);
        _currentIndex++;
    }
    private Color GetSynergyColor(int current, int max)
    {
        if (max == 4) // 일반적인 경우 (2/3/4단계)
        {
            if (current >= 4) return Color.cyan; // 플래티넘
            if (current >= 3) return Color.yellow; // 골드
            if (current >= 2) return Color.gray; // 실버
        }
        else // 4명 고정 시너지 (ex: 어둠 시너지)
        {
            if (current >= max) return Color.cyan; // 4명 딱 되면 플래티넘
        }

        return Color.white; // 그 외 기본색
    }
    //------------------ 시너지 효과 적용 -----------------
    private void ApplyHpBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null || ch.characterType != CharacterType.Tanker) continue;

            ch.MaxHp *= 1 + ratio;
            ch.Hp = ch.MaxHp; 
            Debug.Log("🛡️ 전열 시너지 발동!"+ch.name+ ratio);
        }
    }

    private void ApplyAttackBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null|| ch.synergyType != SynergyType.Dark) continue;

            ch.Damage *= 1 + ratio;
        }
    }

    private void ApplyAllStatsBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null || ch.synergyType != SynergyType.Kingdom) continue;
            ch.MaxHp *= 1 + ratio;
            ch.Hp = ch.MaxHp;
            ch.Damage *= 1 + ratio;
            ch.AttackSpeed *= 1 + ratio;
            ch.Vampiric += ch.Vampiric * ratio;
            ch.manaGain += ch.defaultManaGain * ratio;
        }
    }
    private void ApplyAttackSpeedBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null) continue;
            ch.AttackSpeed *= 1 + ratio;
            Debug.Log("🛡️ 궁수 시너지 발동!");

        }
    }
    private void ApplyVampiricBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null || ch.characterType != CharacterType.Warrior) continue;
            ch.Vampiric += ratio;
            Debug.Log("🛡️ 전사 시너지 발동!" + ch.name + ratio);
        }
    }
    private void ApplyManaBuff(float ratio)
    {
        foreach (GameObject obj in Manager.Battle.characterList)
        {
            Character ch = obj.GetComponent<Character>();
            if (ch == null) continue;
            ch.manaGain = ch.defaultManaGain + ratio;
        }
    }

    //  IEnumerator LightHeal(float healPercent) 얘는 SynergyHelper.cs에 있음 

    public void ResetAndReevaluateSynergies(List<GameObject> characterList) //시너지 효과 제거
    {
        RemoveAllSynergyBuffs();
        ClearSynergyUI();  // 아이콘 제거
        EvaluateSynergies(characterList);  // 다시 조건 체크 후 적용
    }
    private void RemoveAllSynergyBuffs()
    {

            foreach (GameObject obj in Manager.Battle.characterList)
        {
            if (obj == null)
                {
                    Debug.LogWarning("⚠️ characterList 안에 null 오브젝트 있음");
                    continue;
                }

                Character ch = obj.GetComponent<Character>();
                if (ch == null)
                {
                    Debug.LogWarning($"⚠️ {obj.name}에 Character 컴포넌트 없음");
                    continue;
                }
                ch = obj.GetComponent<Character>();
            if (ch == null) continue;

            ch.MaxHp = ch.DefaultMaxHp;
            ch.Damage = ch.DefaultDamage;
            ch.AttackSpeed = ch.DefaultAttackSpeed;
            ch.Vampiric = 0f;
            ch.manaGain = ch.defaultManaGain;
            northWard = 0f;
            SynergyHelper.Instance.StopHeal();

        }
    }
    private void ClearSynergyUI()
    {
        foreach (var slot in _synergySlots)
        {
            slot.gameObject.SetActive(false); // 'SetActive'는 GameObject에 정의되어 있으므로 gameObject를 사용해야 합니다.
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0); // 안 보이게 처리
        }

        _usedIcons.Clear();
        _currentIndex = 0;
    }
    void SynergyUpdate()
    {
        if (Manager.Battle.characterList == null || Manager.Battle.characterList.Count == 0)
        {
            Debug.LogError("❗ characterList가 비어 있거나 null입니다.");
            return;
        }
        Manager.Synergy.ResetAndReevaluateSynergies(Manager.Battle.characterList);
    }

}