using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatDmgText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private RectTransform rect;

    public void Init(float damage, Vector3 worldPos, Transform canvas)
    {
        text = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();

        // 텍스트 세팅
        text.text = Mathf.FloorToInt(damage).ToString();
        text.fontSize = Mathf.Lerp(20f, 50f, Mathf.Clamp01(damage / 1000f));
        text.color = GetColorByDamage(damage);


        // 캔버스 위치 계산
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.SetParent(canvas);
        transform.position = screenPos;

        // DOTween 애니메이션
        rect.DOAnchorPosY(rect.anchoredPosition.y + 100f, 1f).SetEase(Ease.OutQuad);
        transform.DOScale(Vector3.one * 1.2f, 0.3f).SetLoops(2, LoopType.Yoyo);

        text.DOFade(0f, 1f).SetEase(Ease.InOutSine).OnComplete(() => Destroy(gameObject));
    }

    private Color GetColorByDamage(float dmg)
    {
        if (dmg < 1000) return Color.white;
        else if (dmg < 10000) return Color.yellow;
        else if (dmg < 100000) return new Color(0.8f, 0.2f, 1f); // 보라
        else return Color.red;
    }
}
