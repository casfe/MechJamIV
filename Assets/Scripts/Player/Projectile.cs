using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Vector3 forwardDirection = Vector3.forward;
    [SerializeField] Vector3 verticalDirection = Vector3.zero;


    [SerializeField] float forwardVelocity = 100;
    [SerializeField] float verticalVelocity = 10;

    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = new Vector3(0, verticalDirection.y * verticalVelocity, forwardDirection.z * forwardVelocity);
    }

}
