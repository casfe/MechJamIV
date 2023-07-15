using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float velocity = 100;

    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = Vector3.forward * velocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
