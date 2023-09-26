using FMOD;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.LightAnchor;
using Debug = UnityEngine.Debug;

public class PlayerMovement : MonoBehaviour
{
    enum MovementType { Run, Slide, VerticalJump, VerticalFall, JumpToWall, WallRun, JumpAcross, JumpFromWall}

    #region variable declaration

    #region public variables
    [Header("Lane Positions")]    
    [SerializeField] Transform leftLane;
    [SerializeField] Transform rightLane;
    [SerializeField] Transform middleLane;
    [SerializeField] Transform leftWall, rightWall;
    [SerializeField] Transform middleWallPoint;

    [Header("Ground and Wall Detection")]
    [SerializeField] Transform groundedPoint;
    [SerializeField] Transform leftSidePoint;
    [SerializeField] Transform rightSidePoint;
    [SerializeField] float downwardRaycastLength = 0.2f;
    [SerializeField] float sidewaysRaycastLength = 1.0f;

    [Header("Movement Variables")]
    [SerializeField] float horizontalSpeed = 10;
    [SerializeField] float jumpSpeed = 9.8f;
    [SerializeField] float jumpHeight = 2;
    [SerializeField] float sidewaysJumpSpeed = 10;
    [SerializeField] float fallSpeed = 9.8f;
    #endregion

    #region private variables
    Transform currentLane;
    Transform targetLane;
    //Vector3 targetLane;
    private int position;

    //This float gets 1 or -1 depending of what side player is going
    float direction = 1;

    private Vector3 origin;
    private bool halfwayReached = false;
    private bool leftPressed, rightPressed, upPressed, downPressed, verticalPressed, horizontalPressed;

    [Header("Debugging")]
    [SerializeField] private bool isGrounded;   

    #region componenets
    Animator animator;
    CharacterController controller;
    #endregion

    // other variables
    bool appliedMovement = false;
    MovementType state;
    private Vector3 jumpDirection;
    private bool inWallJumpZone = false;
    //private Position targetLane = Position.MiddleLane;
    #endregion

    #endregion

    #region start and update
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        origin = transform.position;
        state = MovementType.Run;

        currentLane = middleLane;
        targetLane = currentLane;
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
            case MovementType.JumpAcross: UpdateWallJump(); break;
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
            if(leftPressed && targetLane == rightWall)
            {
                if(WallOnOppositeSide())
                {
                    SetState(MovementType.JumpAcross);
                }
                else
                {
                    direction = -1;
                    SetState(MovementType.JumpFromWall);
                }
                
            }

            if(rightPressed && targetLane == leftWall)
            {
                if (WallOnOppositeSide())
                {
                    SetState(MovementType.JumpAcross);
                }
                else
                {
                    direction = 1;
                    SetState(MovementType.JumpFromWall);
                }
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

            case MovementType.VerticalFall:
                animator.SetTrigger("Fall");
            break;

            case MovementType.JumpToWall:
                animator.SetTrigger("Jump");
            break;

            case MovementType.WallRun:
                if (targetLane == leftWall)
                    animator.SetTrigger("LeftWall");

                else if (targetLane == rightWall)
                    animator.SetTrigger("RightWall");

                appliedMovement = false;
                break;

            case MovementType.JumpFromWall:
                animator.SetTrigger("Jump");

                if (direction > 0)
                {
                    targetLane = leftLane;
                }
                else if (direction < 0)
                {
                    targetLane = rightLane;
                }

                break;

            case MovementType.JumpAcross:
                animator.SetTrigger("Jump");

                if (targetLane == leftWall)
                {
                    targetLane = rightWall;
                    direction = 1;
                }
                else if (targetLane == rightWall)
                {
                    targetLane = leftWall;
                    direction = -1;            
                }

                origin = transform.position;
                halfwayReached = false;
            break;
        }
    }

    // Moves the player across lanes on the on the road whenever the lane changes
    // Once the player has reached the lane it stops moving
    private void UpdateRun()
    {
        if(!isGrounded)
        {
            SetState(MovementType.VerticalFall);
            return;
        }

        //Perform movement
        if (transform.position.x != targetLane.position.x)
        {
             transform.position = new Vector3(transform.position.x + horizontalSpeed * direction * Time.deltaTime,
                    transform.position.y, transform.position.z);
        }
        else
        {
            appliedMovement = false;
        }

        if (direction > 0 && transform.position.x > targetLane.position.x) // moving right
        {            
            transform.position = new Vector3(targetLane.position.x, transform.position.y, transform.position.z);
        }
        else if (direction < 0 && transform.position.x < targetLane.position.x) // moving left
        {            
            transform.position = new Vector3(targetLane.position.x, transform.position.y, transform.position.z);
        }
    }

    // Sets the new lane on the roade that the player moves to when pressing left or right
    private void ChangeLane(int direction)
    {
        if (position <= -1 && direction < 0)
            return;

        if (position >= 1 && direction > 0)
            return;

        this.direction = direction;
        position += direction;

        switch(position)
        {
            case -1:
                targetLane = leftLane;
                break;

            case 0:
                targetLane = middleLane;
                break;

            case 1:
                targetLane = rightLane;
                break;
        }
    }

    // Moves the player upwards when performing a vertical jump
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

        if (isGrounded)
            SetState(MovementType.Run);
    }

    // Moves the player from the road to to one of the wall point transforms
    private void GroundToWallJump()
    {
        if (targetLane == leftLane)
        {
            targetLane = leftWall;
            jumpDirection = leftWall.position - leftLane.position;
        }
        else if (targetLane == rightLane)
        {
            targetLane = rightWall;
            jumpDirection = rightWall.position - rightLane.position;
        }

        jumpDirection = targetLane.position - transform.position;
        jumpDirection.Normalize();

        if (OnLeftWall() || OnRightWall())
        {
            SetState(MovementType.WallRun);           
        }
        else
        {
            controller.Move(jumpDirection * sidewaysJumpSpeed * Time.deltaTime);
        }

    }

    // Moves the player off the building back onto the nearsest lane on the road
    private void WallToGroundJump()
    {
        jumpDirection = targetLane.position - transform.position;
        jumpDirection.Normalize();

        controller.Move(jumpDirection * sidewaysJumpSpeed * Time.deltaTime);

        if(IsGrounded())
        {
            // update the lane position number
            if (targetLane == leftLane)
                position = -1;
            else if (targetLane == rightLane)
                position = 1;

            SetState(MovementType.Run);
        }
    }
    
    private void UpdateWallRun()
    {
        if(!OnLeftWall() && !OnRightWall())
        {            
            animator.SetTrigger("Fall");
            SetState(MovementType.VerticalFall);
        }
    }

    // Moves the player across from the wall on one side to the opposite wall
    private void UpdateWallJump()
    {
        if (!halfwayReached)
        {
            jumpDirection = middleWallPoint.position - transform.position;

            if (Vector3.Distance(transform.position, middleWallPoint.position) < 0.5f)
                halfwayReached = true;
        }
        else
        {
            jumpDirection = targetLane.position - transform.position;
        }

        jumpDirection.Normalize();

        controller.Move(jumpDirection * sidewaysJumpSpeed * Time.deltaTime);

        if (OnLeftWall() || OnRightWall())
            SetState(MovementType.WallRun);
        else if (direction == 1 && transform.position.x > targetLane.position.x)
            SetState(MovementType.VerticalFall);
        else if (direction == -1 && transform.position.x < targetLane.position.x)
            SetState(MovementType.VerticalFall);
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

    private bool WallOnOppositeSide()
    {
        RaycastHit hitInfo;

        if (targetLane == leftWall)
        {
            if (Physics.Raycast(rightSidePoint.position, Vector3.right, out hitInfo))                
            {
                if (hitInfo.transform.gameObject.layer == 9) // Wall layer
                    return true;
            }            
        }
        else if (targetLane == rightWall)
        {
            if (Physics.Raycast(leftSidePoint.position, Vector3.left, out hitInfo))
            {
                if (hitInfo.transform.gameObject.layer == 9) // Wall layer
                    return true;
            }
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