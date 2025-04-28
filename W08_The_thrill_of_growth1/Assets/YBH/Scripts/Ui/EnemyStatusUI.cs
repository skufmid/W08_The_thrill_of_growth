using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class EnemyStatusUI : MonoBehaviour
{
    Enemy _enemy = null;
    [Header("UI 텍스트")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text damageUIText;
    public TMP_Text damageText;
    public Slider hpSlider;
    public Slider mpSlider;

    private void Start()
    {
        StartCoroutine(CoSetEnemyUI());
    }

    IEnumerator CoSetEnemyUI()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            SetEnemyUI(_enemy);
        }
    }


    public void SetEnemyUI(Enemy enemy)
    {
        _enemy = enemy;
        if (enemy == null || enemy.isActiveAndEnabled == false)
        {
            nameText.text = "";
            hpText.text = "";
            mpText.text = "";
            damageUIText.text = "";
            damageText.text = "";

            hpSlider.value = 0;
            mpSlider.value = 0;
            return;
        }

        nameText.text = enemy.Name;
        hpText.text = $"{enemy.Hp} / {enemy.MaxHp}";
        mpText.text = $"{enemy.Mp} / {enemy.MaxMp}";

        hpSlider.value = (float)enemy.Hp / enemy.MaxHp;
        mpSlider.value = (float)enemy.Mp / enemy.MaxMp;

        damageUIText.text = "공격력";
        damageText.text = $"{enemy.Damage:F1}";
    }
}
