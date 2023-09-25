using FMOD;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

public class PlayerMovement : MonoBehaviour
{
    enum Position { 
        LeftLane = -1, 
        MiddleLane = 0, 
        RightLane = 1, 
        LeftWall = -2, 
        RightWall = 2
    }

    enum MovementType { Run, Slide, VerticalJump, VerticalFall, JumpToWall, WallRun, JumpAcross, JumpFromWall}

    #region variable declaration
    [Header("Lane positions")]
    [SerializeField] Transform rightPoint;
    [SerializeField] Transform leftPoint;
    [SerializeField] Transform middlePoint;

    [Header("Raycast points")]
    [SerializeField] Transform groundedPoint;
    [SerializeField] Transform leftSidePoint;
    [SerializeField] Transform rightSidePoint;

    #region Auxiliar variables
    //Left = 0; Middle = 1; Right = 2
    int actualPosition = 1;
    #endregion

    #region movement Variables
    [Header("Movement Variables")]
    [SerializeField] float horizontalSpeed = 10;
    [SerializeField] float jumpSpeed = 9.8f;
    [SerializeField] float jumpHeight = 2;
    [SerializeField] float sidewaysJumpSpeed = 10;
    [SerializeField] float fallSpeed = 9.8f;
    float targetX;
    //This float gets 1 or -1 depending of what side player is going
    float side = 1;

    private Vector3 origin;

    private bool leftPressed, rightPressed, upPressed, downPressed, verticalPressed, horizontalPressed;

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
    bool appliedMovement = false;
    MovementType state;
    private Vector3 jumpDirection;
    private bool inWallJumpZone = false;
    private Position currentLane = Position.MiddleLane;
    #endregion

    #region start and update
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        origin = transform.position;
        state = MovementType.Run;
    }

    private void Update()
    {
        if (!GameManager.Instance.GameRunning)
            return;

        // perform ground check
        isGrounded = IsGrounded();
        animator.SetBool("onGround", isGrounded);

        UpdateInput();

        switch (state)
        {
            case MovementType.Run: UpdateRun(); break;
            case MovementType.Slide: break;
            case MovementType.VerticalJump: UpdateJump(); break;
            case MovementType.VerticalFall: UpdateFall(); break;
            case MovementType.JumpToWall: GroundToWallJump(); break;
            case MovementType.WallRun: UpdateWallRun(); break;
            case MovementType.JumpFromWall: WallToGroundJump(); break;
        }           
        
    }
    #endregion

    #region private custom functions
    // Get the input from the player and applies actions based on the input
    private void UpdateInput()
    {
        horizontalPressed = Input.GetAxisRaw("Horizontal") != 0;
        verticalPressed = Input.GetAxisRaw("Vertical") != 0;        

        leftPressed = Input.GetAxisRaw("Horizontal") < 0;
        rightPressed = Input.GetAxisRaw("Horizontal") > 0;
        upPressed = Input.GetAxisRaw("Vertical") > 0;
        downPressed = Input.GetAxisRaw("Vertical") < 0;

        if (state == MovementType.Run && !appliedMovement)
        {
            if (leftPressed) 
                ChangeLane(-1);
            else if (rightPressed) 
                ChangeLane(1);
            else if (upPressed)
            {
                if (inWallJumpZone)
                    SetState(MovementType.JumpToWall);
                else
                    SetState(MovementType.VerticalJump);
            }
            else if (downPressed)
                SetState(MovementType.Slide);
        }
        else if(state == MovementType.WallRun)
        {
            if(leftPressed && currentLane == Position.RightWall)
            {
                side = -1;
                SetState(MovementType.JumpFromWall);
            }

            if(rightPressed && currentLane == Position.LeftWall)
            {
                side = 1;
                SetState(MovementType.JumpFromWall);
            }
        }

        // prevent additional input until all keys are realeased
        if (verticalPressed || horizontalPressed)
            appliedMovement = true;        
    }

    // Set the type of player movement in the finite state machine
    private void SetState(MovementType newState)
    {
        if (newState == state)
            return;

        state = newState;

        // initialize the new state
        switch(state)
        {
            case MovementType.Run:
                appliedMovement = false;
                break;

            case MovementType.Slide:
                controller.height /= 2;
                controller.center = new Vector3(0, -0.5f, 0);

                animator.SetTrigger("Slide");
                break;

            case MovementType.VerticalJump:
                animator.SetTrigger("Jump");
            break;

            case MovementType.JumpToWall:
                animator.SetTrigger("Jump");
            break;

            case MovementType.WallRun:
                if (jumpDirection == Vector3.left)
                    animator.SetTrigger("LeftWall");

                else if (jumpDirection == Vector3.right)
                    animator.SetTrigger("RightWall");

                appliedMovement = false;
                break;

            case MovementType.JumpFromWall:
                    animator.SetTrigger("Jump");
                break;
        }
    }

    private void UpdateRun()
    {
        //Perform movement
        if (targetX != transform.position.x)
        {
             transform.position = new Vector3(transform.position.x + horizontalSpeed * side * Time.deltaTime,
                    transform.position.y, transform.position.z);
        }
        else
        {            
            appliedMovement = false;
        }

        if (side > 0) // moving right
        {
            if (transform.position.x > targetX)
                transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        }
        else if (side < 0) // moving left
        {
            if (transform.position.x < targetX)
                transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }
    }

    private void ChangeLane(int direction)
    {
        if ((int) currentLane <= -1 && direction < 0)
            return;

        if ((int)currentLane >= 1 && direction > 0)
            return;

        side = direction;
        currentLane += direction;

        switch(currentLane)
        {
            case Position.LeftLane:
                targetX = leftPoint.position.x;
                break;

            case Position.MiddleLane:
                targetX = middlePoint.position.x;
                break;

            case Position.RightLane: 
                targetX = rightPoint.position.x;
                break;
        }
    }

    private void StartWallRun(Vector3 direction)
    {
        if (direction == Vector3.left)
            animator.SetTrigger("LeftWall");
        else if (direction == Vector3.right)
            animator.SetTrigger("RightWall");

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
            SetState(MovementType.VerticalFall);
        }
    }

    // Pushed player down towards ground
    private void UpdateFall()
    {
        controller.Move(Vector3.down * fallSpeed * Time.deltaTime);

        if (IsGrounded())
            SetState(MovementType.Run);
    }

    private void GroundToWallJump()
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

        if (OnLeftWall() || OnRightWall())
        {
            Debug.Log("Hit Wall");
            SetState(MovementType.WallRun);           
        }
        else
        {
            controller.Move(velocity * Time.deltaTime);
        }

    }

    // Moves the player off the building back onto the road
    private void WallToGroundJump()
    {
        Vector3 velocity = new Vector3();

        velocity += Vector3.down * fallSpeed;

        if (side > 0)
            velocity += Vector3.right * horizontalSpeed;
        else if (side < 0)
            velocity += Vector3.left * horizontalSpeed;

        controller.Move(velocity * Time.deltaTime);

        if(IsGrounded())
        {
            SetState(MovementType.Run);
        }
    }

    private void UpdateWallRun()
    {
        if(!OnLeftWall() && !OnRightWall())
        {            
            animator.SetTrigger("Fall");

            Debug.Log("Off the wall");
            return;
        }

        Debug.Log("Running on wall");

    }

    #region Ground and wall checks
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
    /// Rentuns collider to the original height and changes back to running state
    /// </summary>
    public void StandUp()
    {
        controller.height *= 2;
        controller.center = Vector3.zero;
        //Ducking = false;

        SetState(MovementType.Run);
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