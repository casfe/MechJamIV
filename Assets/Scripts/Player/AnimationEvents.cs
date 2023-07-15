using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void Die()
    {
        GameManager.Instance.EndGame();
    }
}
