using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO", order = 2)]
public class SkillSO : ScriptableObject
{
    [Header("스킬 정보")]
    public int Id;
    public string Name;
    public Sprite sprite;
    public Animation animation;

    public float[] Coefficients;

    [TextArea] public string SkillDescription;

}
