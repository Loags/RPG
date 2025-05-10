using UnityEngine;


namespace LB
{
    public class PlayerLocomotion : MonoBehaviour
    {
        // private PlayerController playerController;
        // private Transform cameraObject;
        // private InputHandler inputHandler;
        // public Vector3 moveDirection;
        //
        // [HideInInspector] public Transform myTransform;
        // [HideInInspector] public AnimatorHandler animatorHandler;
        //
        // public new Rigidbody rigidbody;
        // public GameObject normalCamera;
        //
        // [Header("Ground & Air Detection Stats")]
        // [SerializeField] float groundDetectionRayStartPoint;
        // [SerializeField] float minimungDistanceNeededToBeginFall;
        // [SerializeField] float groundDirectionRayDistance;
        // private LayerMask ignoreForGroundCheck;
        // public float inAirTimer;
        //
        // [Space(20)]
        // [Header("Movement Stats")]
        // [SerializeField] private float movementSpeed;
        // [SerializeField] private float sprintSpeed;
        // [SerializeField] private float rotationSpeed;
        // [SerializeField] private float fallingSpeed;
        //
        // void Start()
        // {
        //     playerController = PlayerController.instance;
        //     rigidbody = GetComponent<Rigidbody>();
        //     inputHandler = GetComponent<InputHandler>();
        //     animatorHandler = GetComponentInChildren<AnimatorHandler>();
        //     cameraObject = Camera.main.transform;
        //     myTransform = transform;
        //     animatorHandler.Initialize();
        //
        //     playerController.isGrounded = true;
        //     ignoreForGroundCheck = ~(1 << 10);
        // }
        //
        // #region Movement
        // private Vector3 normalVector = Vector3.zero;
        // private Vector3 targetPosition;
        //
        // public void HandleMoveSprint(float _delta)
        // {
        //     if (playerController.blockMovement) return;
        //
        //     if (playerController.isInteracting) return;
        //
        //     moveDirection = cameraObject.forward * inputHandler.vertical;
        //     moveDirection += cameraObject.right * inputHandler.horizontal;
        //     moveDirection.Normalize();
        //     moveDirection.y = 0;
        //
        //     float speed = movementSpeed;
        //
        //     if (playerController.isSprinting && inputHandler.moveAmount > 0.5f)
        //     {
        //         speed = sprintSpeed;
        //         playerController.isSprinting = true;
        //         moveDirection *= speed;
        //     }
        //     else
        //     {
        //         if (inputHandler.moveAmount < 0.5f)
        //         {
        //             moveDirection *= movementSpeed;
        //             playerController.isSprinting = false;
        //         }
        //         else
        //         {
        //             moveDirection *= speed;
        //             playerController.isSprinting = false;
        //         }
        //
        //     }
        //
        //     Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        //     rigidbody.velocity = projectedVelocity;
        //
        //     animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerController.isSprinting);
        //
        //     if (animatorHandler.canRotate)
        //     {
        //         HandleRotation(_delta);
        //     }
        // }
        // private void HandleRotation(float _delta)
        // {
        //     Vector3 targetDir = Vector3.zero;
        //     float moveOverride = inputHandler.moveAmount;
        //
        //     targetDir = cameraObject.forward * inputHandler.vertical;
        //     targetDir += cameraObject.right * inputHandler.horizontal;
        //
        //     targetDir.Normalize();
        //     targetDir.y = 0;
        //
        //     if (targetDir == Vector3.zero)
        //         targetDir = myTransform.forward;
        //
        //     float rs = rotationSpeed;
        //
        //     Quaternion tr = Quaternion.LookRotation(targetDir);
        //     Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * _delta);
        //
        //     myTransform.rotation = targetRotation;
        // }
        //
        // public void HandleFalling(float _delta, Vector3 _moveDirection)
        // {
        //     playerController.isGrounded = false;
        //     RaycastHit hit;
        //     Vector3 origin = myTransform.position;
        //     origin.y += groundDetectionRayStartPoint;
        //
        //     if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        //     {
        //         _moveDirection = Vector3.zero;
        //     }
        //
        //     if (playerController.isInAir)
        //     {
        //         rigidbody.AddForce(-Vector3.up * fallingSpeed);
        //         rigidbody.AddForce(_moveDirection * fallingSpeed / 8f); // Jump of the edge to not get stuck
        //     }
        //
        //     Vector3 dir = _moveDirection;
        //     dir.Normalize();
        //     origin += dir * groundDirectionRayDistance;
        //
        //     targetPosition = myTransform.position;
        //
        //     Debug.DrawRay(origin, -Vector3.up * minimungDistanceNeededToBeginFall, Color.red, 0.1f, false);
        //
        //     if (Physics.Raycast(origin, -Vector3.up, out hit, minimungDistanceNeededToBeginFall, ignoreForGroundCheck))
        //     {
        //         normalVector = hit.normal;
        //         Vector3 tp = hit.point;
        //         playerController.isGrounded = true;
        //         targetPosition.y = tp.y;
        //
        //         if (playerController.isInAir)
        //         {
        //             if (inAirTimer > 0.5f)
        //             {
        //                 animatorHandler.PlayTargetAnimation("Land", true);
        //                 inAirTimer = 0;
        //             }
        //             else
        //             {
        //                 animatorHandler.PlayTargetAnimation("Empty", false);
        //                 inAirTimer = 0;
        //             }
        //
        //             playerController.isInAir = false;
        //         }
        //     }
        //     else
        //     {
        //         if (playerController.isGrounded)
        //         {
        //             playerController.isGrounded = false;
        //         }
        //
        //         if (playerController.isInAir == false)
        //         {
        //             if (playerController.isInteracting == false)
        //             {
        //                 animatorHandler.PlayTargetAnimation("Falling", true);
        //             }
        //
        //             Vector3 vel = rigidbody.velocity;
        //             vel.Normalize();
        //             rigidbody.velocity = vel * (movementSpeed / 2);
        //             playerController.isInAir = true;
        //         }
        //     }
        //
        //     if (playerController.isInteracting || inputHandler.moveAmount > 0)
        //     {
        //         myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
        //     }
        //     else
        //     {
        //         myTransform.position = targetPosition;
        //     }
        // }
        // #endregion
    }
}