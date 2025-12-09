using System;
using _Project.Core.Scripts;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.Locomotion
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform cameraTransform;

        [Header("Input Actions")] [SerializeField]
        private PlayerInputHandler inputActions;

        [Header("Movement")] [SerializeField] private float movementSpeed;
        [SerializeField] private float maxMovementSpeed;
        [SerializeField] private float turnYSpeed;
        [SerializeField] private bool shouldFaceMovementDirection = false;

        [Header("Jump")] [SerializeField] private float jumpVelocity;
        [SerializeField] private bool jumpAirControl;
        [SerializeField] private float jumpAirControlMultiplier;
        [Range(-2f, 2f)] [SerializeField] private float groundCheckYOffset = 0.5f;
        [SerializeField] private float checkSphereRadius;
        [SerializeField] private LayerMask groundMask;
        public bool isGrounded;
        private Vector3 lastMoveDir;

        // private PlayerInputActions inputActions;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        private void Start()
        {
        }

        private void Update()
        {
            GroundCheck();
            // Polling way of getting input
            // bool jump = inputActions.Player.Jump.WasPressedThisFrame();

            // I prefer event-flag-based approach
            // bool jumpPressed = input.JumpPressed;

            // if (isGrounded && jumpPressed)
            // {
            //     Jump();
            //     inputActions.ConsumeJump();
            // }
        }

        private void LateUpdate()
        {
            Vector2 lookRot = inputActions.Look;
        }

        void FixedUpdate()
        {
            HandleMovement();
        }


        private void HandleMovement()
        {
            Vector2 move = inputActions.Move;

            if (move.sqrMagnitude < 0.0001f || !isGrounded)
                return;

            var moveDir = CamNormalize(move);

            Vector3 targetPos = rb.position + moveDir * (movementSpeed * Time.fixedDeltaTime);
            rb.MovePosition(targetPos);


            if (shouldFaceMovementDirection)
            {
                HandleRotation(moveDir);
            }
        }

        private void HandleRotation(Vector3 moveDir)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            Quaternion smoothedRotation =
                Quaternion.Slerp(rb.rotation, targetRotation, turnYSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }


        private void GroundCheck()
        {
            Vector3 checkPos = rb.position + new Vector3(0f, -groundCheckYOffset, 0f);
            isGrounded = Physics.CheckSphere(checkPos, checkSphereRadius, groundMask);
        }

        private void Jump()
        {
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }

        private Vector3 CamNormalize(Vector2 move)
        {
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            Vector3 moveDir = camForward * move.y + camRight * move.x;
            moveDir.Normalize();
            return moveDir;
        }

        private void OnDrawGizmos()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
                if (rb == null) return;
            }

            Vector3 checkPos = rb.position + new Vector3(0f, -groundCheckYOffset, 0f);
            Gizmos.color = isGrounded ? Color.green : Color.red;

            Gizmos.DrawSphere(checkPos, checkSphereRadius);
        }
    }
}