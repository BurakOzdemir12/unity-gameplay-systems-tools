using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Core.Scripts
{
    public class PlayerInputHandler : MonoBehaviour, PlayerInputActions.IPlayerActions
    {
        private PlayerInputActions inputActions;
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action JumpEvent;
        public event Action DodgeEvent;

        // public bool JumpPressed { get; private set; }

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);
        }

        private void OnEnable()
        {
            inputActions.Enable();
            //using interface so setCallback does the job.
            // inputActions.Player.Jump.performed += ctx => Jump = true; you can use this too
            // inputActions.Player.Jump.performed += OnJump;
        }


        private void OnDisable()
        {
            // inputActions.Player.Jump.performed -= OnJump;
            inputActions.Disable();
        }

        private void OnDestroy()
        {
            inputActions.Player.Disable();
        }

        void Update()
        {
            Move = inputActions.Player.Move.ReadValue<Vector2>();
            Look = inputActions.Player.Look.ReadValue<Vector2>();
        }

        // public void ConsumeJump()
        // {
        //     JumpPressed = false;
        // }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            JumpEvent?.Invoke();
            // JumpPressed = true;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            DodgeEvent?.Invoke();
        }
    }
}