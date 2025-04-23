using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO", order = 1)]
public class CharacterSO : ScriptableObject
{
    [Header("기본 정보")]
    public int Id;
    public string Name;
    public EAlliance Alliance;
    public EClass Class;
    public Sprite sprite;

    [Header("능력치")]
    public float BaseHP;
    public float HPPerLevel;
    public float MP;
    public float BaseDamage;
    public float DamagePerLevel;
    public float AttackSpeed;
}