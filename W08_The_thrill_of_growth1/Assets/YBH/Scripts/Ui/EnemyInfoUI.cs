using UnityEngine;
using TMPro;

public class EnemyInfoUI : MonoBehaviour
{
    [Header("UI 텍스트")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text damageText;

    public void SetEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("⚠️ Enemy is null");
            return;
        }

        nameText.text = enemy.name;
        hpText.text = $"{enemy.Hp} / {enemy.MaxHp}";
        mpText.text = $"{enemy.Mp} / {enemy.MaxMp}";
        damageText.text = $"{enemy.Damage:F1}";
    }
}
