using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO", order = 1)]
public class CharacterSO : ScriptableObject
{
    [Header("기본 정보")]
    public int Id;
    public string Name;
    public SynergyManager.SynergyType SynergyType;
    public SynergyManager.CharacterType CharacterType;
    public GameObject Prefabs;

    [Header("능력치")]
    public float BaseHP;
    public float HPPerLevel;
    public float MP;
    public float BaseDamage;
    public float DamagePerLevel;
    public float AttackSpeed;
}