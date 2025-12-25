using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates
{
    public class PlayerGroundedState : PlayerBaseState
    {
        private float ungroundedTime;

        public PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.InputHandler.DodgeEvent += OnDodge;
            stateMachine.InputHandler.RollEvent += OnRoll;
            stateMachine.InputHandler.JumpEvent += OnJump;

            if (stateMachine.Targeter.SelectedTarget != null)
            {
                SetSubState(new PlayerTargetingState(stateMachine));
                return;
            }

            SetSubState(new PlayerFreeLookState(stateMachine));
        }


        public override void Tick(float deltaTime)
        {
            if (stateMachine.Controller.isGrounded)
            {
                ungroundedTime = 0f;
                return;
            }

            ungroundedTime += deltaTime;
            if (ungroundedTime >= stateMachine.GroundedGrace)
                SwitchRootState(new PlayerAirborneState(stateMachine));
        }


        public override void Exit()
        {
            stateMachine.InputHandler.DodgeEvent -= OnDodge;
            stateMachine.InputHandler.RollEvent -= OnRoll;
            stateMachine.InputHandler.JumpEvent -= OnJump;
            
            ClearSubState();
        }

        // public wrapper for childstates
        public void SwitchSubState(PlayerBaseState newSubState) => SetSubState(newSubState);

        private void OnDodge()
        {
            if (Time.time - stateMachine.PreviousDodgeTime < stateMachine.DodgeCooldownTime)
            {
                return;
            }

            stateMachine.SetDodgeCooldownTime(Time.time);
            SetSubState(new PlayerDodgeState(stateMachine));
            // stateMachine.SwitchState(new PlayerDodgeState(stateMachine));
        }

        private void OnRoll()
        {
            if (!stateMachine.inAlertMode) return;

            if (Time.time - stateMachine.PreviousRollTime < stateMachine.RollCooldownTime)
            {
                return;
            }

            stateMachine.SetRollCooldownTime(Time.time);
            SetSubState(new PlayerRollState(stateMachine));
            // stateMachine.SwitchState(new PlayerRollState(stateMachine));
        }

        private void OnJump()
        {
            if (stateMachine.inAlertMode) return;
            if (Time.time - stateMachine.PreviousJumpTime < stateMachine.JumpCooldownTime)
            {
                return;
            }

            SetSubState(new PlayerJumpingState(stateMachine));
            // stateMachine.SwitchState(new PlayerRollState(stateMachine));
        }
    }
}