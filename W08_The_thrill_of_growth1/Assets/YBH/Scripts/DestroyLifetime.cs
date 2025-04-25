using UnityEngine;
using System.Collections;

public class DestroyLifetime : MonoBehaviour
{
    public float Lifetime;

    void Start()
    {
        Invoke("Destroy", Lifetime);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
