using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Data/CharacterSynergy")]
public class CharacterDataSO : ScriptableObject
{
    public SynergyManager.CharacterType charactertType;
    public string displayName;
    public Sprite icon;
    public string description;

}
