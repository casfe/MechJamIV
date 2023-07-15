using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public UnityEvent OnCollideWithObstacle;

    Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

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
            OnCollideWithObstacle.Invoke();
            //GameObject.Destroy(collision.gameObject);
            //GameManager.Instance.EndGame();
            animator.SetTrigger("Die");
            GetComponent<PlayerMovement>().enabled = false;
            this.enabled = false;
        }
    }

    private void FireWeapon()
    {

    }
}