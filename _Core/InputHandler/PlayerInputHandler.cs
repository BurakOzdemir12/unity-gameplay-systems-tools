using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace _Project.Systems._Core.InputHandler
{
    public class PlayerInputHandler : MonoBehaviour, PlayerInputActions.IPlayerActions
    {
        private PlayerInputActions inputActions;
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action JumpEvent;
        public event Action DodgeEvent;
        public event Action RollEvent;
        public event Action TargetEvent;
        public event Action TargetCancelEvent;
        public event Action RollOrJumpEvent;
        public event Action InteractEvent;
        public event Action DrawSheathEvent;
        public event Action InventoryEvent;
        public event Action ParryEvent;
        public bool IsAttacking { get; private set; }
        public bool IsBlocking { get; private set; }
        public bool IsSprinting { get; private set; }

        #region Hotbars

        public event Action<int> HotbarSelectEvent;
        public event Action<int> HotbarScrollEvent;

        // public event Action Hotbar1Event;
        // public event Action Hotbar2Event;
        // public event Action Hotbar3Event;
        // public event Action Hotbar4Event;
        // public event Action Hotbar5Event;
        // public event Action Hotbar6Event;
        // public event Action Hotbar7Event;
        // public event Action Hotbar8Event;
        // public event Action Hotbar9Event;
        // public event Action Hotbar00Event;

        #endregion

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

        public void OnTarget(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (context.interaction is TapInteraction)
            {
                TargetEvent?.Invoke();
            }
        }

        public void OnTargetCancel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (context.interaction is HoldInteraction)
            {
                TargetCancelEvent?.Invoke();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsAttacking = true;
            }

            if (context.canceled)
            {
                IsAttacking = false;
            }
        }

        public void OnBlock(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsBlocking = true;
            }

            if (context.canceled)
            {
                IsBlocking = false;
            }
        }

        public void OnParry(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (IsBlocking)
            {
                ParryEvent?.Invoke();
            }
        }

        public void OnRoll(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            RollEvent?.Invoke();
        }

        public void OnRollOrJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            RollOrJumpEvent?.Invoke();
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            InteractEvent?.Invoke();
        }

        public void OnDrawSheath(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            DrawSheathEvent?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsSprinting = true;
            }

            if (context.canceled)
            {
                IsSprinting = false;
            }
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            InventoryEvent?.Invoke();
        }

        public void OnHotbar1(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar1Event?.Invoke();
            HotbarSelectEvent?.Invoke(0);
        }

        public void OnHotbar2(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar2Event?.Invoke();
            HotbarSelectEvent?.Invoke(1);
        }

        public void OnHotbar3(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar3Event?.Invoke();
            HotbarSelectEvent?.Invoke(2);
        }

        public void OnHotbar4(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar4Event?.Invoke();
            HotbarSelectEvent?.Invoke(3);
        }

        public void OnHotbar5(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar5Event?.Invoke();
            HotbarSelectEvent?.Invoke(4);
        }

        public void OnHotbar6(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar6Event?.Invoke();
            HotbarSelectEvent?.Invoke(5);
        }

        public void OnHotbar7(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar7Event?.Invoke();
            HotbarSelectEvent?.Invoke(6);
        }

        public void OnHotbar8(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar8Event?.Invoke();
            HotbarSelectEvent?.Invoke(7);
        }

        public void OnHotbar9(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar9Event?.Invoke();
            HotbarSelectEvent?.Invoke(8);
        }

        public void OnHotbar10(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            // Hotbar00Event?.Invoke();
            HotbarSelectEvent?.Invoke(9);
        }

        public void OnMouseWheel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            float value = context.ReadValue<float>();
            if (Mathf.Abs(value) < 0.01f) return;
            HotbarScrollEvent?.Invoke(value > 0 ? -1 : +1);
        }
    }
}