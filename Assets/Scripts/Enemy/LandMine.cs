using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
            GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 3) // ground layer
        {
            transform.parent = collision.transform;
        }
    }
}
