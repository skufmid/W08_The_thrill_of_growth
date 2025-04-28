using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    public Transform[] transforms;
    public GameObject Enemy;
    public GameObject Boss;
    int BossNumber = 0; // 보스 번호
    private void Start()
    {
        Manager.Game.OnStartStage += SpawnEnemies;
    }

    public void SpawnEnemies()
    {
        if (Manager.Game.stageNum % 10 == 0) // 스테이지에 따라 보스 소환
        {
            Debug.Log("보스 소환");
            GameObject bossInstance = Instantiate(Boss, transforms[5]); // 보스 인스턴스 생성

            // 보스의 Enemy 컴포넌트를 가져와 bossCheck 호출
            Enemy bossEnemy = bossInstance.GetComponent<Enemy>();
            if (bossEnemy != null)
            {
                bossEnemy.bossCheck(BossNumber); // bossCheck 이벤트 호출
            }
            else
            {
                Debug.LogError("❗ 보스 오브젝트에 Enemy 컴포넌트가 없습니다.");
            }
            if (BossNumber < 3)
            {
                BossNumber++;
            }
            else
            {
                BossNumber = 0;
            }
        }
        else
        {
            // transforms 배열에서 랜덤하게 7개 위치를 선택
            int spawnCount = Mathf.Min(7, transforms.Length);
            // Fisher-Yates shuffle로 랜덤 인덱스 섞기
            Transform[] shuffled = (Transform[])transforms.Clone();
            for (int i = 0; i < shuffled.Length; i++)
            {
                int rand = Random.Range(i, shuffled.Length);
                var temp = shuffled[i];
                shuffled[i] = shuffled[rand];
                shuffled[rand] = temp;
            }
            for (int i = 0; i < spawnCount; i++)
            {
                Debug.Log("적 소환");
                Instantiate(Enemy, shuffled[i]);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Manager.Game.StartStage();
        }

    }
}
