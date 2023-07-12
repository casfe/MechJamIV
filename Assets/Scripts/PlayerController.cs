using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FireWeapon();
        }
    }
    
    /*
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Hit " + hit.gameObject.name);

        if(hit.gameObject.layer == 6) // hazard layer
        {
            GameManager.Instance.EndGame();
        }
    }*/
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6) // hazard layer
        {
            GameObject.Destroy(collision.gameObject);
            GameManager.Instance.EndGame();
        }
    }

    private void FireWeapon()
    {

    }
}