using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private CharacterCanvas characterCanvas;

    private void Awake()
    {
        characterCanvas = FindAnyObjectByType<CharacterCanvas>();
    }

    private void Start()
    {
        
    }
    public void SetCharacterUI(Character character)
    {
        characterCanvas.SetCharacter(character);
    }
}
