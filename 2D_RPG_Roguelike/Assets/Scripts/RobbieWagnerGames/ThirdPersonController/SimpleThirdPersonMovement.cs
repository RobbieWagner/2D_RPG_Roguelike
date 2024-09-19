using RobbieWagnerGames.FirstPerson;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames
{
    public class SimpleThirdPersonMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform referenceCam;
        [SerializeField] private float speed = 6f;
        [SerializeField] private float turnSmoothTime = 0.1f;
        private float turnVel;
        [SerializeField] private PlayerMovementActions controls;

        private bool isMoving = false;

        private Vector3 inputVector = Vector3.zero;

        [Header("Grounding and Gravity")]
        private bool isGrounded = false;
        private float GRAVITY = -9.8f;
        [SerializeField] private LayerMask groundMask;

        private GroundType currentGroundType = GroundType.None;
        public GroundType CurrentGroundType
        {
            get { return currentGroundType; }
            set
            {
                if (currentGroundType == value)
                    return;

                currentGroundType = value;
            }
        }

        private bool canMove = true;
        public bool CanMove
        {
            get { return canMove; }
            set
            {
                if (value == canMove) return;

                canMove = value;
                OnToggleMovement?.Invoke(canMove);
            }
        }

        public delegate void ToggleDelegate(bool on);
        public event ToggleDelegate OnToggleMovement;

        public static SimpleThirdPersonMovement Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            SetupControls();
        }

        private void SetupControls()
        {
            controls = new PlayerMovementActions();
            controls.Movement.Move.performed += OnMove;
            controls.Movement.Move.canceled += OnStop;
            OnToggleMovement += ToggleMovement;
            if (CanMove)
                controls.Movement.Enable();
        }

        private void OnStop(InputAction.CallbackContext context)
        {
            isMoving = false;
            inputVector = Vector3.zero;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            Vector3 direction = new Vector3(input.x, 0 ,input.y).normalized;

            if (direction.magnitude >= 0.1f)
            {
                isMoving = true;
                inputVector = direction;
            }
            else
            {
                isMoving = false;
                inputVector = Vector3.zero;
            }

        }

        private void LateUpdate()
        {
            UpdateGroundCheck();

            if (inputVector.magnitude >= 0.1f && isMoving)
            {
                float targetAngle = Mathf.Atan2(inputVector.x, inputVector.z) * Mathf.Rad2Deg + referenceCam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVel, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                if (characterController.enabled)
                    characterController.Move(moveDirection.normalized * speed * Time.deltaTime);
            }

        }

        private void UpdateGroundCheck()
        {
            RaycastHit hit;
            isGrounded = Physics.Raycast(transform.position + new Vector3(0, .01f, 0), Vector3.down, out hit, .1f, groundMask);

            if (hit.collider != null)
            {
                GroundInfo groundInfo = hit.collider.gameObject.GetComponent<GroundInfo>();
                if (groundInfo != null)
                    CurrentGroundType = groundInfo.groundType;
                else
                    CurrentGroundType = GroundType.None;
            }

            if (!isGrounded)
                inputVector.y += GRAVITY * Time.deltaTime;
            else
                inputVector.y = 0f;
        }

        private void ToggleMovement(bool on)
        {
            if (on)
                controls.Movement.Enable();
            else
                controls.Movement.Disable();
        }
    }
}