using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textPrefab;
    public Transform canvas;

    public void ShowDamage(float damage, Vector3 worldPos)
    {
        GameObject go = Instantiate(textPrefab, canvas);
        go.GetComponent<FloatDmgText>().Init(damage, worldPos, canvas);
    }
}