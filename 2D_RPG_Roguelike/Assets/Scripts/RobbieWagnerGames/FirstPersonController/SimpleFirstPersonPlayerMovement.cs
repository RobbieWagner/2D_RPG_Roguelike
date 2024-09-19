using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.FirstPerson
{
    public class SimpleFirstPersonPlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
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
        private GroundType currentGroundType = GroundType.None;
        public GroundType CurrentGroundType
        {
            get { return currentGroundType; }
            set
            {
                if (currentGroundType == value)
                    return;

                currentGroundType = value;
                //if (AudioEventsLibrary.Instance.groundTypeSounds.ContainsKey(currentGroundType))
                //    StartCoroutine(ChangeFootstepSounds(currentGroundType));
                //else
                //    StartCoroutine(ChangeFootstepSounds());
            }
        }
        public delegate void ToggleDelegate(bool on);
        public event ToggleDelegate OnToggleMovement;

        [HideInInspector] public bool isMoving = false;
        [SerializeField] private float initialSpeed = 5f;
        private float currentSpeed;
        private Vector3 inputVector = Vector3.zero;
        private PlayerMovementActions inputActions;

        [Header("Physics Components")]
        [SerializeField] private CharacterController characterController;

        [Header("Grounding and Gravity")]
        private bool isGrounded = false;
        //[SerializeField] private float groundCheckDistance = 3f;
        private float GRAVITY = -9.8f;
        //private float TERMINAL_VELOCITY = -100f;
        //private float currentYVelocity = 0f;

        [SerializeField] private LayerMask groundMask;

        public static SimpleFirstPersonPlayerMovement Instance { get; private set; }

        // Start is called before the first frame update
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

            currentSpeed = initialSpeed;

            SetupControls();
        }

        private void SetupControls()
        {
            inputActions = new PlayerMovementActions();
            inputActions.Movement.Move.performed += OnMove;
            inputActions.Movement.Move.canceled += OnStop;
            OnToggleMovement += ToggleMovement;
            if (CanMove)
                inputActions.Movement.Enable();
        }

        private void ToggleMovement(bool on)
        {
            if (on)
                inputActions.Movement.Enable();
            else
                inputActions.Movement.Disable();
        }

        private void LateUpdate()
        {
            UpdateGroundCheck();

            Vector3 movementVector = transform.right * inputVector.x + transform.forward * inputVector.z + Vector3.up * inputVector.y;

            if (characterController.enabled)
                characterController.Move(movementVector * currentSpeed * Time.deltaTime);

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

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (inputVector.x != input.x && input.x != 0f)
            {
                isMoving = true;
            }
            else if (input.x == 0 && inputVector.z != input.y && input.y != 0f)
            {
                isMoving = true;
            }
            else if (input.x == 0 && input.y == 0)
            {
                isMoving = false;
            }

            inputVector.x = input.x;
            inputVector.z = input.y;

            //Debug.Log($"input change: {input} processed: {inputVector} ");
        }

        private void OnStop(InputAction.CallbackContext context)
        {
            inputVector = Vector3.zero;
            isMoving = false;
        }
    }
}