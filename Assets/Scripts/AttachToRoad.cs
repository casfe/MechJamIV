using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToRoad : MonoBehaviour
{
    [SerializeField] Vector3 castingOffset;

    // Start is called before the first frame update
    void Start()
    {
        if(Physics.SphereCast(transform.position + castingOffset, 0.1f, Vector3.down, out RaycastHit hit))
        {
            transform.parent = hit.transform;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + castingOffset, 0.1f);
    }

}
