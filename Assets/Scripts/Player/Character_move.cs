using UnityEngine;

public class Character_move : MonoBehaviour
{
    [Header("- - - - [Markers] - - - -")]

    [SerializeField] GameObject rightPoint;
    [SerializeField] GameObject leftPoint, middlePoint;


    #region Auxiliar variables
    //Left = 0; Middle = 1; Right = 2
    int actualPosition = 1;
    #endregion

    #region movementVariables
    [Header("- - - - [Movement] - - - -")]
    [SerializeField] float speed;
    float targetX;
    //This float gets 1 or -1 depending of what side player is going
    float side = 1;
    #endregion

    private void Update()
    {
        //Check if is moving
        if(targetX == transform.position.x)
        { 
            //Check if key pressed
            //TODO: KeyConfig
            if (Input.GetKeyDown(KeyCode.A)) goLeft();
            if (Input.GetKeyDown(KeyCode.D)) goRight();
        }

        //Perform movement
        if (targetX != transform.position.x)
        {
            transform.position = new Vector3(transform.position.x + speed * side * Time.deltaTime,transform.position.y,transform.position.z);
        }

        //Check if reached and put player in target EXACTLY position
        if (Mathf.Abs(targetX - transform.position.x) < 0.1)
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

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
                moveLeft();
                break;
            case 2:
                side = 1;
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
                side = -1;
                actualPosition = 2;
                targetX = rightPoint.transform.position.x;
                moveRight();
                break;
            case 0:
                side = -1;
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
    { }

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

}