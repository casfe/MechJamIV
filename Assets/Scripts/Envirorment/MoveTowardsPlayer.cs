using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour
{
    private float speed;

    private void Start()
    {
        speed = GameManager.Instance.RunSpeed;
    }

    void Update()
    {
        if (GameManager.Instance.GameRunning)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
            GameObject.Destroy(gameObject);
    }

}
