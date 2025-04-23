using UnityEngine;

[CreateAssetMenu(fileName = "SynergyDataSO", menuName = "Data/Synergy")]
public class SynergyDataSO : ScriptableObject
{
    public SynergyManager.SynergyType synergyType;
    public string displayName;
    public Sprite icon;
    public string description;
}