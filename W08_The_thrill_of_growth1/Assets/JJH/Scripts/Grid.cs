using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    public Transform[] transforms;
    public GameObject Enemy;
    private void Start()
    {
        Manager.Game.OnStartStage += SpawnEnemies;
    }

    public void SpawnEnemies()
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Manager.Game.StartStage();
        }

    }
}
