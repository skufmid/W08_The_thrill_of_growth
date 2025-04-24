using UnityEngine;
using System.Collections;

public class DestroyLifetime : MonoBehaviour
{
    public float Lifetime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Destroy", Lifetime);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Destroy()
    {
        Destroy(gameObject);
    }
}
