using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO", order = 3)]
public class EnemySO : ScriptableObject
{
    [Header("기본 정보")]
    public int Id;
    public string Name;
    public Sprite sprite;

    [Header("능력치")]
    public float BaseHP;
    public float MP;
    public float BaseDamage;
}
