using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10;
    [SerializeField] bool moveOnSpawn = false;

    private bool moving;

    void Start()
    {
        moving = moveOnSpawn;
    }

    void Update()
    {
        if(moving)
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
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

        if(collision.gameObject.tag == "Player")
        {
            GameObject.Destroy(gameObject);
        }
    }
}
