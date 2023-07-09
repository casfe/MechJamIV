using UnityEngine;

public class Character_move : MonoBehaviour
{
    public CharacterController cc;
    [SerializeField] private float speed = 100f;

    public float Gravity = -9.81f;
    public Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask floorMask;
    public bool isGrounded;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform cam;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }
    void Update()
    {
        //Movimiento
        float horizontal = 0;
        float vertical = 1;

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        //Detección de suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, floorMask);
        //Gravedad
        velocity.y += Gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
        //Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = -2 * Gravity;
            Gravity = Gravity * -1;
            groundCheck.transform.localPosition = new Vector3(groundCheck.transform.localPosition.x, groundCheck.transform.localPosition.y * -1);
        }

    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, 2);
    }

}