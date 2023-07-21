using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Character_move : MonoBehaviour
{
    [Header("- - - - [Markers] - - - -")]

    [SerializeField] GameObject rightPoint;
    [SerializeField] GameObject leftPoint, middlePoint;
    [SerializeField] GameObject leftBuildingPoint, RightBuildingPoint;

    public enum BuildJumpingStartPosition { LEFT, RIGHT, ZERO };

    #region Auxiliar variables
    //Left = 0; Middle = 1; Right = 2
    int actualPosition = 1;
    #endregion

    #region movementVariables
    [Header("- - - - [Movement] - - - -")]
    [SerializeField] float speed;
    [SerializeField] float speedY;
    float targetX;
    float targetY = 0;
    //This float gets 1 or -1 depending of what side player is going
    float side = 1;
    bool buildJumping;
    #endregion

    private void Start()
    {
        targetY = transform.position.y;
    }


    private void Update()
    {
        //test purpouses change from road to builds
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!buildJumping)
            {
                buildJumping = true;
                speed = speed * 2;
                BuildJumping(BuildJumpingStartPosition.RIGHT);
            }
            else
            {
                buildJumping = false;
                speed = speed / 2;
                if (transform.position == leftBuildingPoint.transform.position)
                    side = -1;
                else 
                    side = 1;
                actualPosition = 1;
                targetX = middlePoint.transform.position.x;
                targetY = 2.3f;
                StartCoroutine(RotatePlayer(BuildJumpingStartPosition.ZERO,2));
            }
        
        }

        //Check Type of movement
        if (buildJumping)
        {
            //Key to jump to the other building
            //TODO: KeyConfig
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                //Check player position
                if (transform.position == leftBuildingPoint.transform.position)
                    BuildJumping(BuildJumpingStartPosition.RIGHT);
                else if(transform.position == RightBuildingPoint.transform.position)
                    BuildJumping(BuildJumpingStartPosition.LEFT);
            }
        }
        //If not BuildJumping
        else 
        { 
        //Check if is moving
        if(targetX == transform.position.x)
        { 
            //Check if key pressed
            //TODO: KeyConfig
            if (Input.GetKeyDown(KeyCode.D)) goLeft();
            if (Input.GetKeyDown(KeyCode.A)) goRight();
        }

        }

        //Perform  T movement
        if (targetY != transform.position.y)
        {
            if (targetY < transform.position.y)
                transform.position = new Vector3(transform.position.x, transform.position.y - speedY * Time.deltaTime, transform.position.z);
            else
               transform.position = new Vector3(transform.position.x, transform.position.y + speedY * Time.deltaTime, transform.position.z);
        }

        //Check if reached and put player in target EXACTLY position
        if (Mathf.Abs(targetY - transform.position.y) < 0.1)
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
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
                side = 1;
                actualPosition = 0;
                targetX = leftPoint.transform.position.x;
                targetY = 2.3f;
                break;
            case 2:
                side = 1;
                actualPosition = 1;
                targetX = middlePoint.transform.position.x;
                targetY = 2.3f;
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
                side = -1;
                actualPosition = 2;
                targetX = rightPoint.transform.position.x;
                targetY = 2.3f;
                break;
            case 0:
                side = -1;
                actualPosition = 1;
                targetX = middlePoint.transform.position.x;
                targetY = 2.3f;
                break;
        }
    }
    /// <summary>
    /// Speeds up the player
    /// </summary>
    /// <param name="value">Value to add to speed</param>
    public void speedUp(float value)
    {
        speed += value;
    }

    /// <summary>
    /// Slows down the player
    /// </summary>
    /// <param name="value">Value to substract to speed</param>
    public void slowDown(float value)
    {
        speed -= value;
    }

    /// <summary>
    /// Makes the player positioning on the buildJumping desired position
    /// </summary>
    /// <param name="pos">Start wall</param>
    public void BuildJumping(BuildJumpingStartPosition pos, int secondsToAnim = 2)
    {
        if (pos == BuildJumpingStartPosition.LEFT) 
        {
            targetX = leftBuildingPoint.transform.position.x;
            targetY = leftBuildingPoint.transform.position.y;
            side = 1;
            StartCoroutine(RotatePlayer(BuildJumpingStartPosition.LEFT, secondsToAnim));
        }
        else
        {
            side = -1;
            targetX = RightBuildingPoint.transform.position.x;
            targetY = RightBuildingPoint.transform.position.y;
            StartCoroutine(RotatePlayer(BuildJumpingStartPosition.RIGHT, secondsToAnim));
        }    
    }

    IEnumerator RotatePlayer(BuildJumpingStartPosition pos, int seconds)
    {
        float targetRotation = 0;
        if (pos == BuildJumpingStartPosition.LEFT) targetRotation = 90;
        else if (pos == BuildJumpingStartPosition.RIGHT) targetRotation = -90;
        else targetRotation = 0;


        float rotSide = 1;
        if (transform.rotation.eulerAngles.z == 0 && pos == BuildJumpingStartPosition.RIGHT) rotSide = -1;


        float degreesToRotate = transform.rotation.eulerAngles.z - targetRotation;

        //Fix
        if (degreesToRotate == 270) { degreesToRotate = -90; rotSide = -1; }
        if(degreesToRotate == 90 && pos == BuildJumpingStartPosition.ZERO) rotSide = -1;    


            Debug.Log(degreesToRotate);
        //Store the angle to rotate for each second
        float rotateAngle = degreesToRotate / 10 / seconds;
        for (int i = 0; i < seconds * 10; i++)
        {
            transform.eulerAngles = new Vector3 (transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z + (rotateAngle * rotSide));
            yield return new WaitForSeconds(0.1f);
        }
        //Rotationfinished
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,targetRotation);
    }

    IEnumerator RotatePlayerRight() 
    {
        yield return new WaitForSeconds(0.1f);
    }

}