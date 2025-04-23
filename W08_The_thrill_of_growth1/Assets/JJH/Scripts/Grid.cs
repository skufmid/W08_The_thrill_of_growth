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
        foreach (Transform t in transforms)
        {
            Debug.Log("적 소환");
            Instantiate(Enemy, t);

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
