using FMOD;
using System.Net.NetworkInformation;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerMovement : MonoBehaviour
{
    [Header("- - - - [Markers] - - - -")]

    [SerializeField] GameObject rightPoint;
    [SerializeField] GameObject leftPoint, middlePoint;
    [SerializeField] Transform groundedPoint;

    #region Auxiliar variables
    //Left = 0; Middle = 1; Right = 2
    int actualPosition = 1;
    #endregion

    #region movementVariables
    [Header("- - - - [Movement] - - - -")]
    [SerializeField] float horizontalSpeed = 10;
    [SerializeField] float jumpSpeed = 9.8f;
    [SerializeField] float jumpHeight = 2;
    [SerializeField] float fallSpeed = 9.8f;
    //[SerializeField] float duckTime;
    float targetX;
    //This float gets 1 or -1 depending of what side player is going
    float side = 1;

    private Vector3 origin;

    private bool leftPressed, rightPressed, upPressed, downPressed;

    [SerializeField] bool isGrounded;
    #endregion

    #region componenets
    Animator animator;
    CharacterController controller;
    #endregion

    #region properties
    public bool Jumping { get; set; } = false;
    public bool Ducking { get; private set; } = false;
    #endregion

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

            //Check if is moving
            if (targetX == transform.position.x)
            {
                //Check if key pressed
                if (leftPressed)
                    goLeft();
                else if (rightPressed)
                    goRight();
                else if (upPressed && !Jumping && !Ducking && isGrounded)
                    Jump();
                else if (downPressed && !Ducking & !Jumping && isGrounded)
                    Duck();
            }

            PerformHorizontalMovement();
        }

        // fall if in air
        if (!isGrounded)
        {            
            controller.Move(Vector3.down * fallSpeed * Time.deltaTime);

            if (isGrounded)
            {
                Jumping = false;                
            }
        }

        animator.SetBool("onGround", true);

        if (Jumping)
            UpdateJump();

        isGrounded = IsGrounded();
    }

    private void PerformHorizontalMovement()
    {
        //Perform movement
        if (targetX != transform.position.x)
        {
            transform.position = new Vector3(transform.position.x + horizontalSpeed * side * Time.deltaTime, transform.position.y, transform.position.z);
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
                moveLeft();
                break;
            case 2:
                side = -1;
                actualPosition = 1;
                targetX = middlePoint.transform.position.x; 
                moveLeft();
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
                moveRight();
                break;
            case 0:
                side = 1;
                actualPosition = 1;
                targetX = middlePoint.transform.position.x;
                moveLeft();
                break;
        }
    }

    /// <summary>
    /// Starts player movement to right
    /// </summary>
    private void moveRight()
    { 
    
    
    }
    /// <summary>
    /// Starts player movement to left
    /// </summary>
    private void moveLeft()
    {
        
    }    

    private void Jump()
    {
        //Debug.Log("Jump triggered");
        animator.SetTrigger("Jump");
        Jumping = true;
    }

    private void Duck()
    {
        controller.height /= 2;
        controller.center = new Vector3(0, -0.5f, 0);


        animator.SetTrigger("Slide");
        Ducking = true;
    }

    private void UpdateJump()
    {
        if(transform.position.y <= (origin.y + jumpHeight))
            controller.Move(Vector3.up * jumpSpeed * Time.deltaTime);
        else
        {
            Jumping = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(groundedPoint.position, Vector3.down, 0.2f);
    }

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

    public void StandUp()
    {
        controller.height *= 2;
        controller.center = Vector3.zero;
        Ducking = false;
    }
    #endregion region

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * jumpHeight, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(groundedPoint.position, Vector3.down * 0.1f);
    }
}