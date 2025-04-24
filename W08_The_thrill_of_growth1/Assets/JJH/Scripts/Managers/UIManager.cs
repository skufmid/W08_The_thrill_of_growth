using UnityEngine;

public class UIManager
{
    public FloatingTextManager floatingTextManager;

    public void Init()
    {
        // 씬에서 오브젝트를 찾아 연결
        floatingTextManager = GameObject.FindAnyObjectByType<FloatingTextManager>();
        if (floatingTextManager == null)
            Debug.LogError("⚠️ FloatingTextManager를 찾지 못했습니다.");
        else
        {
            Debug.Log("FloatingTextManager를 찾았습니다.");
        }
    }
}
