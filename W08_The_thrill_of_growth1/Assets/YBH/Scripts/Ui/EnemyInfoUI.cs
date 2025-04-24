using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyInfoUI : MonoBehaviour
{
    Enemy _enemy = null;
    [Header("UI 텍스트")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text damageUIText;
    public TMP_Text damageText;

    private void Start()
    {
        StartCoroutine(CoSetEnemyUI());
    }

    IEnumerator CoSetEnemyUI()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            SetEnemyUI(_enemy);
        }
    }


    public void SetEnemyUI(Enemy enemy)
    {
        _enemy = enemy;
        if (enemy == null || enemy.enabled == false)
        {
            nameText.text = "";
            hpText.text = "";
            mpText.text = "";
            damageUIText.text = "";
            damageText.text = "";
            return;
        }

        nameText.text = enemy.Name;
        hpText.text = $"{enemy.Hp} / {enemy.MaxHp}";
        mpText.text = $"{enemy.Mp} / {enemy.MaxMp}";
        damageUIText.text = "데미지";
        damageText.text = $"{enemy.Damage:F1}";
    }
}
