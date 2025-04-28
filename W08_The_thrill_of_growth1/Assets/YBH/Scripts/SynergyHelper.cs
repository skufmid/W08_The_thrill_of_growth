using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyHelper : MonoBehaviour
{
    public static SynergyHelper Instance;
    private Coroutine healRoutine;
    private void Awake()
    {
        Instance = this;

    }

    public void RunCoroutine(float healPercent)
    {
        if (healRoutine != null)
            StopCoroutine(healRoutine);

        healRoutine = StartCoroutine(LightHeal(healPercent));
    }

    public IEnumerator LightHeal(float healPercent)
    {

        while (true)
        {
            if (!Manager.Battle.isInBattle)
            {
                yield return null; // 전투 중이 아니면 다음 프레임까지 대기
                continue;
            }

            yield return new WaitForSeconds(2f);
            List<GameObject> characters = Manager.Battle.characterList;
            Character lowestHpChar = null;
            float minHpRatio = float.MaxValue;

            foreach (GameObject obj in characters)
            {
                if (obj == null) continue;

                Character ch = obj.GetComponent<Character>();
                if (ch == null) continue;

                float hpRatio = ch.Hp / ch.MaxHp;
                if (hpRatio < minHpRatio)
                {
                    minHpRatio = hpRatio;
                    lowestHpChar = ch;
                }
            }

            if (lowestHpChar != null)
            {
                float amount = lowestHpChar.MaxHp * healPercent;
                lowestHpChar.Hp = Mathf.Min(lowestHpChar.Hp + amount, lowestHpChar.MaxHp);
                lowestHpChar.HealEffectActive();
            }
        }
    }
    public void StopHeal()
    {
        if (healRoutine != null)
        {
            StopCoroutine(healRoutine);
            healRoutine = null;
        }
    }

}