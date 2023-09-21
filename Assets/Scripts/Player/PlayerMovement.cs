using FMOD;
using System.Net.NetworkInformation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

public class PlayerMovement : MonoBehaviour
{
    enum Position { LeftLane, MiddleLane, RightLane, LeftWall, RightWall}
    enum MovementType { Running, Sliding, VerticalJump, SidewaysJump, JumpDownward}

    #region variable declaration
    [Header("- - - - [Markers] - - - -")]
    [SerializeField] GameObject rightPoint;
    [SerializeField] GameObject leftPoint, middlePoint;
    [SerializeField] Transform groundedPoint;
    [SerializeField] Transform leftSidePoint;
    [SerializeField] Transform rightSidePoint;

    #region Auxiliar variables
    //Left = 0; Middle = 1; Right = 2
    int actualPosition = 1;
    #endregion

    #region movementVariables
    [Header("- - - - [Movement] - - - -")]
    [SerializeField] float horizontalSpeed = 10;
    [SerializeField] float jumpSpeed = 9.8f;
    [SerializeField] float jumpHeight = 2;
    [SerializeField] float sidewaysJumpSpeed = 10;
    [SerializeField] float fallSpeed = 9.8f;
    //[SerializeField] float duckTime;
    float targetX;
    //This float gets 1 or -1 depending of what side player is going
    float side = 1;

    private Vector3 origin;

    private bool leftPressed, rightPressed, upPressed, downPressed;

    [Header("Debugging")]
    [SerializeField] bool isGrounded;
    [SerializeField] float downwardRaycastLength = 0.2f;
    [SerializeField] float sidewaysRaycastLength = 1.0f;
    #endregion

    #region componenets
    Animator animator;
    CharacterController controller;
    #endregion

    // other variables
    private Vector3 jumpDirection;
    // flags
    private bool inWallJumpZone = false;
    private bool sidewaysJump = false;
    private bool wallRunning = false;
    private bool wallJumping = false;
    private Position currentLane = Position.MiddleLane;
    #endregion

    #region properties
    public bool Jumping { get; set; } = false;
    public bool Ducking { get; private set; } = false;
    #endregion

    #region start and update
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        origin = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance.GameRunning)
        {
            leftPressed = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
            rightPressed = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
            upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
            downPressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
            
            // Check if the player is currently still on a lane
            if (targetX == transform.position.x && !Jumping && !Ducking && isGrounded)
            {
                //Check if key pressed
                if (leftPressed)
                {
                    goLeft();
                }
                else if (rightPressed)
                    goRight();
                else if (upPressed)
                    Jump();
                else if (downPressed)
                    Duck();
            }
            else if (wallRunning) // if the player is on a wall
            {
                if (leftPressed && currentLane == Position.RightWall)
                {
                    wallRunning = false;
                    wallJumping = true;
                    side = -1;
                    animator.SetTrigger("Jump");
                    animator.SetTrigger("Fall");
                }

                if (rightPressed && currentLane == Position.LeftWall)
                {
                    wallRunning = false;
                    wallJumping = true;
                    side = 1;
                    animator.SetTrigger("Jump");
                    animator.SetTrigger("Fall");
                }
            }

            isGrounded = IsGrounded();

            if (!Jumping && !Ducking && isGrounded)
                PerformHorizontalMovement();

            // fall if in air and not attached to a building
            if (!isGrounded && !wallRunning)
            {
                if (wallJumping)
                {
                    Vector3 velocity = new Vector3();

                    velocity += Vector3.down * fallSpeed;

                    if (side > 0)
                        velocity += Vector3.right * horizontalSpeed;
                    else if (side < 0)
                        velocity += Vector3.left * horizontalSpeed;

                    controller.Move(velocity * Time.deltaTime);
                }
                else
                {
                    controller.Move(Vector3.down * fallSpeed * Time.deltaTime);
                }

                if (isGrounded)
                {
                    Jumping = false;
                    wallJumping = false;
                }
            }

            animator.SetBool("onGround", isGrounded);

            if (Jumping)
            {
                if (sidewaysJump)
                    UpdateSidewaysJump();
                else
                    UpdateJump();
            }

            if (wallRunning)
                UpdateWallRun();
        }
    }
    #endregion

    #region private custom functions
    private void PerformHorizontalMovement()
    {
        //Perform movement
        if (targetX != transform.position.x)
        {
             transform.position = new Vector3(transform.position.x + horizontalSpeed * side * Time.deltaTime,
                    transform.position.y, transform.position.z);
        }
        else
        {
            currentLane = (Position) actualPosition;
        }

        if (side > 0) // moving right
        {
            if (transform.position.x >= targetX)
                transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        }
        else if (side < 0) // moving left
        {
            if (transform.position.x <= targetX)
                transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// Manages player pushing A button to move left.
    /// </summary>
    private void goLeft() 
    {
        switch(actualPosition) 
        {
            default:
            case 0: break;
            case 1:
                side = -1;
                actualPosition = 0;
                targetX = leftPoint.transform.position.x;                
                break;
            case 2:
                side = -1;
                actualPosition = 1;
                targetX = middlePoint.transform.position.x;                
                break;
        }
    }

    /// <summary>
    /// Manages player pushing D button to move right
    /// </summary>
    private void goRight()
    {
        switch (actualPosition)
        {
            default:
            case 2: break;
            case 1:
                side = 1;
                actualPosition = 2;
                targetX = rightPoint.transform.position.x;
                break;
            case 0:
                side = 1;
                actualPosition = 1;
                targetX = middlePoint.transform.position.x;
                break;
        }
    }

    private void Jump()
    {
        //Debug.Log("Jump triggered");
        animator.SetTrigger("Jump");
        Jumping = true;

        if (inWallJumpZone)
            sidewaysJump = true;
    }

    private void Duck()
    {
        controller.height /= 2;
        controller.center = new Vector3(0, -0.5f, 0);

        animator.SetTrigger("Slide");
        Ducking = true;
    }

    private void StartWallRun(Vector3 direction)
    {
        if (direction == Vector3.left)
            animator.SetTrigger("LeftWall");
        else if (direction == Vector3.right)
            animator.SetTrigger("RightWall");

        wallRunning = true;

        Debug.Log("Start of wall run");
    }

    private void UpdateJump()
    {
        // check that we have not yet reached max height
        if (transform.position.y <= (origin.y + jumpHeight))
        {           
            controller.Move(Vector2.up * jumpSpeed * Time.deltaTime); // apply the jump            
        }
        else
        {
            Jumping = false;
        }
    }

    private void UpdateSidewaysJump()
    {
        Vector3 velocity = new Vector3();

        if (currentLane == Position.LeftLane)
        {
            jumpDirection = Vector3.left;
            currentLane = Position.LeftWall;
        }
        else if (currentLane == Position.RightLane)
        {
            jumpDirection = Vector3.right;
            currentLane = Position.RightLane;
        }

        velocity += Vector3.up * jumpSpeed;
        velocity += jumpDirection * sidewaysJumpSpeed;

        //if (IsOnWall(Vector3.left) || IsOnWall(Vector3.right))
        if (OnLeftWall() || OnRightWall())
        {
            Debug.Log("Hit Wall");
            StartWallRun(jumpDirection);
            Jumping = false;            
        }
        else
        {
            controller.Move(velocity * Time.deltaTime);
        }

    }

    private void UpdateWallRun()
    {
        if(!OnLeftWall() && !OnRightWall())
        {
            wallRunning = false;
            animator.SetTrigger("Fall");

            Debug.Log("Off the wall");
            return;
        }

        Debug.Log("Running on wall");
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(groundedPoint.position, Vector3.down, downwardRaycastLength);
    }

    private bool IsOnWall(Vector3 direction)
    {        
        if(Physics.Raycast(groundedPoint.position, direction, out RaycastHit hitInfo, sidewaysRaycastLength))
        {
            if (hitInfo.transform.gameObject.layer == 9) // Wall layer
                return true;
            else
                Debug.Log("Raycast hit " + hitInfo.transform.name);
        }

        return false;
    }

    private bool OnLeftWall()
    {
        if (Physics.Raycast(leftSidePoint.position, Vector3.left, out RaycastHit hitInfo, sidewaysRaycastLength))
        {
            if (hitInfo.transform.gameObject.layer == 9) // Wall layer
                return true;
            else
                Debug.Log("Raycast hit " + hitInfo.transform.name);
        }

        return false;
    }

    private bool OnRightWall()
    {
        if (Physics.Raycast(rightSidePoint.position, Vector3.right, out RaycastHit hitInfo, sidewaysRaycastLength))
        {
            if (hitInfo.transform.gameObject.layer == 9) // Wall layer
                return true;
            else
                Debug.Log("Raycast hit " + hitInfo.transform.name);
        }

        return false;
    }

    #endregion

    #region public functions
    /// <summary>
    /// Speeds up the player
    /// </summary>
    /// <param name="value">Value to add to speed</param>
    public void speedUp(float value)
    {
        horizontalSpeed += value;
    }

    /// <summary>
    /// Slows down the player
    /// </summary>
    /// <param name="value">Value to substract to speed</param>
    public void slowDown(float value)
    {
        horizontalSpeed -= value;
    }

    /// <summary>
    /// Rentuns collider to the original height and clears ducking flag
    /// </summary>
    public void StandUp()
    {
        controller.height *= 2;
        controller.center = Vector3.zero;
        Ducking = false;
    }
    #endregion region

    #region collision and trigger functions
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "JumpZone")
        {
            inWallJumpZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "JumpZone")
        {
            inWallJumpZone = false;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * jumpHeight, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(groundedPoint.position, Vector3.down * downwardRaycastLength);
        Gizmos.DrawRay(leftSidePoint.position, Vector3.left * sidewaysRaycastLength);
        Gizmos.DrawRay(rightSidePoint.position, Vector3.right * sidewaysRaycastLength);
    }
}