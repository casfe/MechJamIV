using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float travelSpeed = 50;
    [SerializeField] float speedToLanePosition = 50;
    [SerializeField] float downwardSpeed = 2.0f;
    [SerializeField] float standbyTime = 1.0f;

    public Vector3 InitialDirection { get; set; }
    public Transform LanePosition { get; set; }

    private Rigidbody rb;
    //private Vector3 initialDirection;
    private bool lanePositionReached = false;
    private bool movingTowardsPlayer = false;
    private float timePassed = 0;

    /*
    public void SetInitialDirection(Vector3 direction)
    {
        initialDirection.x = transform.forward.x * direction.x;
        initialDirection.y = transform.forward.y * direction.y;
        initialDirection.z = transform.forward.z * direction.z;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = InitialDirection * speedToLanePosition;
    }

    // Update is called once per frame
    void Update()
    {      
        /*
        if(!lanePositionReached)
        {
            if(Vector3.Distance(transform.position, LanePosition.position) < 1)
            {
                rb.velocity = Vector3.zero;
                lanePositionReached = true;
            }
        }*/

        if (lanePositionReached && !movingTowardsPlayer)
        {           
            timePassed += Time.deltaTime;

            if (timePassed >= standbyTime)
            {
                rb.velocity = new Vector3(0, -downwardSpeed, -travelSpeed);
                movingTowardsPlayer = true;
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == LanePosition)
        {
            rb.velocity = Vector3.zero;
            lanePositionReached = true;
        }
        else if(other.tag == "Boundary")
        {
            GameObject.Destroy(gameObject);
        }
                
    }
}
