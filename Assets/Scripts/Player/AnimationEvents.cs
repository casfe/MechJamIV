using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    public void EndJump()
    {
        movement.Jumping = false;
    }

    public void EndSlide()
    {
        movement.StandUp();
    }

    public void Die()
    {
        GameManager.Instance.EndGame();
    }
}
