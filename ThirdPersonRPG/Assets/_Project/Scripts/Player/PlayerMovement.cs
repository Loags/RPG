using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    private Transform cam;

    [Header("Movement")]
    public float maxSpeed;
    public float currentSpeed;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float decelerationSpeed;
    private float turnSmoothVelocity;
    private Vector3 lastMoveDir = new();

    [Space(10)]
    [Header("Jump")]
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main.transform;
    }


    void Update()
    {
        //jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) { velocity.y = -5f; }
        if (Input.GetButtonDown("Jump") && isGrounded) { velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity); }

        //gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        //walk
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = new();
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            currentSpeed += Time.deltaTime * accelerationSpeed;
            if (currentSpeed >= maxSpeed)
                currentSpeed = maxSpeed;

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (moveDir != Vector3.zero)
                lastMoveDir = moveDir;

            characterController.Move(currentSpeed * Time.deltaTime * moveDir.normalized);
        }
        else
        {

            if (currentSpeed > 0)
                currentSpeed -= Time.deltaTime * decelerationSpeed;
            else currentSpeed = 0;

            characterController.Move(currentSpeed * Time.deltaTime * lastMoveDir.normalized);
        }
    }
}