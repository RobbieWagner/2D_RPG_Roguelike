using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.FirstPerson
{
    public class SimpleFirstPersonLook : MonoBehaviour
    {

        [SerializeField] private float mouseSensitivity = 500f;
        [SerializeField] private float controllerSpeed = .5f;
        [SerializeField] private Transform playerBody;

        PlayerMovementActions inputActions;

        private float xRotation = 0f;

        private bool usingController = false;
        private Vector2 controllerInput;

        private bool canLook = true;
        public bool CanLook
        {
            get
            {
                return canLook;
            }
            set
            {
                if (value == canLook)
                    return;
                canLook = value;
                if (canLook)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;

                onToggleLookState?.Invoke(canLook);
            }
        }
        public delegate void OnToggleLookState(bool on);
        public event OnToggleLookState onToggleLookState;

        public static SimpleFirstPersonLook Instance { get; private set; }

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

            if (canLook)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;

            inputActions = new PlayerMovementActions();
            inputActions.Movement.Enable();
            inputActions.Movement.MouseLook.performed += OnMouseLook;
            inputActions.Movement.ControllerLook.performed += OnControllerLook;
            inputActions.Movement.ControllerLook.canceled += StopRotating;
        }

        private void Update()
        {
            if (usingController)
            {
                float inputX = controllerInput.x;
                float inputY = controllerInput.y;

                xRotation -= inputY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * inputX);
            }
        }

        private void OnMouseLook(InputAction.CallbackContext context)
        {
            if (CanLook)
            {
                usingController = false;

                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }

        private void OnControllerLook(InputAction.CallbackContext context)
        {
            if (CanLook)
            {
                usingController = true;

                Vector2 input = context.ReadValue<Vector2>();

                if (Mathf.Abs(input.x) < 0.05f && Mathf.Abs(input.y) < 0.05f)
                    StopRotating(context);
                else
                    controllerInput = context.ReadValue<Vector2>() * controllerSpeed;
            }
        }

        private void StopRotating(InputAction.CallbackContext context)
        {
            controllerInput = Vector2.zero;
        }

        public void EnableLook()
        {
            CanLook = true;
        }

        public void DisableLook()
        {
            CanLook = false;
        }
    }
}