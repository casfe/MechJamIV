using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10;
    [SerializeField] bool moveOnSpawn = false;

    private bool moving;
    Vector3 direction;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moving = moveOnSpawn;

        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;

        if (playerPos.z > transform.position.z)
            direction = Vector3.forward;
        else
            direction = Vector3.back;
    }

    void Update()
    {
        if (moving)
            rb.velocity = direction * moveSpeed * Time.deltaTime;
            //transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
            GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 3) // ground layer
        {
            moving = true;
        }

        /*
        if(collision.gameObject.tag == "Player")
        {
            GameObject.Destroy(gameObject);
        }*/
    }
}
