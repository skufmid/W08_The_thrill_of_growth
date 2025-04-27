using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrbSelfPush : MonoBehaviour
{
    RectTransform _rect;
    static List<OrbSelfPush> _allOrbs = new List<OrbSelfPush>();

    float _minDistance = 50f; // 최소 거리 (픽셀 단위)
    float _pushSpeed = 300f;   // 밀어내는 힘 (클수록 빠르게 떨어짐)
    [SerializeField] bool isMovable = true; // 기본은 움직일 수 있음 (Orb), ForbiddenArea는 false로

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _allOrbs.Add(this);
    }

    private void OnDestroy()
    {
        _allOrbs.Remove(this);
    }

    private void Update()
    {
        if (!isMovable) return; // ❗ 나는 안 움직여야 한다면 Update 아예 패스할 수도 있음 (선택)

        foreach (var other in _allOrbs)
        {
            if (other == this) continue;

            Vector3 dir = _rect.position - other._rect.position;
            float dist = dir.magnitude;

            if (dist < _minDistance && dist > 0.01f)
            {
                Vector3 pushDir = dir.normalized;
                float pushStrength = _pushSpeed * Time.deltaTime * (_minDistance - dist) / _minDistance;

                // 👉 서로 모두 movable이면 둘 다 밀어내기
                if (other.isMovable)
                {
                    _rect.position += pushDir * pushStrength * 0.5f;
                    other._rect.position -= pushDir * pushStrength * 0.5f;
                }
                else
                {
                    // 👉 다른 놈이 immovable(=ForbiddenArea)이면, 나만 밀리기
                    _rect.position += pushDir * pushStrength;
                }
            }
        }
    }
}

