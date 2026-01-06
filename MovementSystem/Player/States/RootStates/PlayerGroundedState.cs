using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.ClimbingSystem.States.RootStates;
using _Project.Systems.CombatSystem.Player.States;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Player.States.RootStates
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
            stateMachine.InputHandler.RollOrJumpEvent += OnRollOrJumpEvent;


            if (stateMachine.Targeter.SelectedTarget != null)
            {
                SetSubState(new PlayerTargetingState(stateMachine));
                return;
            }

            SetSubState(new PlayerFreeLookState(stateMachine));
        }


        public override void Tick(float deltaTime)
        {
            if (stateMachine.IsGrounded)
            {
                ungroundedTime = 0f;
                return;
            }

            // // Vector3 rayOrigin = stateMachine.transform.position + Vector3.up * 0.1f;
            // float probeDistance = stateMachine.AirborneHeightThreshold;
            // bool isGroundBelow = Physics.Raycast(
            //     rayOrigin,
            //     Vector3.down,
            //     out RaycastHit hit,
            //     probeDistance,
            //     Physics.DefaultRaycastLayers,
            //     QueryTriggerInteraction.Ignore
            // );
            // ungroundedTime += deltaTime;
            // if (ungroundedTime >= stateMachine.GroundedGrace)
            //     SwitchRootState(new PlayerAirborneState(stateMachine));
            //
            //
            if (stateMachine.GroundChecker != null &&
                stateMachine.GroundChecker.DistanceToGround <= stateMachine.GroundedSnapDistance)
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
            stateMachine.InputHandler.RollOrJumpEvent -= OnRollOrJumpEvent;


            ClearSubState();
        }

        // public wrapper for childstates
        public void SwitchSubState(PlayerBaseState newSubState) => SetSubState(newSubState);

        private void OnDodge()
        {
            if (Time.time - stateMachine.PreviousDodgeTime < stateMachine.PlayerConfigSo.DodgeData.DodgeCooldownTime)
            {
                return;
            }

            stateMachine.SetDodgeCooldownTime(Time.time);
            SetSubState(new PlayerDodgeState(stateMachine));
            // stateMachine.SwitchState(new PlayerDodgeState(stateMachine));
        }

        private void OnRoll()
        {
            if (!stateMachine.IsInAlertMode) return;

            if (Time.time - stateMachine.PreviousRollTime < stateMachine.PlayerConfigSo.RollData.RollCooldownTime)
            {
                return;
            }

            stateMachine.SetRollCooldownTime(Time.time);
            SetSubState(new PlayerRollState(stateMachine));
            // stateMachine.SwitchState(new PlayerRollState(stateMachine));
        }

        private void OnJump()
        {
            if (stateMachine.IsInAlertMode) return;
            if (Time.time - stateMachine.PreviousJumpTime < stateMachine.PlayerConfigSo.JumpData.JumpCooldownTime)
            {
                return;
            }

            stateMachine.PendingJumpState(Time.time);

            SwitchRootState(new PlayerAirborneState(stateMachine));
            // stateMachine.SwitchState(new PlayerRollState(stateMachine));
        }

        private void OnRollOrJumpEvent()
        {
            if (!stateMachine.IsInAlertMode)
            {
                if (Time.time - stateMachine.PreviousJumpTime < stateMachine.PlayerConfigSo.JumpData.JumpCooldownTime)
                {
                    return;
                }

                if (stateMachine.ClimbController != null && stateMachine.ClimbController.HasValidLedge)
                {
                    SwitchRootState(new PlayerClimbState(stateMachine));
                    return;
                }

                stateMachine.PendingJumpState(Time.time);

                SwitchRootState(new PlayerAirborneState(stateMachine));
                // stateMachine.SwitchState(new PlayerRollState(stateMachine));
            }
            else
            {
                if (Time.time - stateMachine.PreviousRollTime < stateMachine.PlayerConfigSo.RollData.RollCooldownTime)
                {
                    return;
                }

                stateMachine.SetRollCooldownTime(Time.time);
                SetSubState(new PlayerRollState(stateMachine));
                // stateMachine.SwitchState(new PlayerRollState(stateMachine));
            }
        }
    }
}