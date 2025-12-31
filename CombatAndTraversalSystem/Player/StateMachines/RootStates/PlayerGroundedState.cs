using _Project.Systems.ClimbingSystem.States.RootStates;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates
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
            if (stateMachine.Controller.isGrounded)
            {
                ungroundedTime = 0f;
                return;
            }

            //TODO Create GroundProbe Script Grounded and Fall Distance Check with Raycast or spehere cast
            //TODO You can add GroundNormal, GroundMaterialType, GroundDistance
            
            Vector3 rayOrigin = stateMachine.transform.position + Vector3.up * 0.1f;
            float probeDistance = stateMachine.AirborneHeightThreshold;
            bool isGroundBelow = Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                probeDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            );
            ungroundedTime += deltaTime;
            if (ungroundedTime >= stateMachine.GroundedGrace && !isGroundBelow)
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
            if (!stateMachine.IsInAlertMode) return;

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
            if (stateMachine.IsInAlertMode) return;
            if (Time.time - stateMachine.PreviousJumpTime < stateMachine.JumpCooldownTime)
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
                if (Time.time - stateMachine.PreviousJumpTime < stateMachine.JumpCooldownTime)
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
                if (Time.time - stateMachine.PreviousRollTime < stateMachine.RollCooldownTime)
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

//
// public override void Tick(float deltaTime)
// {
//     float radius = stateMachine.Controller.radius * 0.9f;
//
//     float groundedProbeDist = Mathf.Max(
//         0.25f,
//         stateMachine.Controller.stepOffset + stateMachine.Controller.skinWidth + 0.05f
//     );
//
//     bool groundedByCC = stateMachine.Controller.isGrounded;
//     bool groundedByProbe = ProbeGroundBelow(out _, groundedProbeDist, radius, upOffset: 0.1f);
//
//     // Grounded kabulü: OR
//     if (groundedByCC || groundedByProbe)
//     {
//         ungroundedTime = 0f;
//         return;
//     }
//
//     ungroundedTime += deltaTime;
//
//     if (ungroundedTime >= stateMachine.GroundedGrace)
//         SwitchRootState(new PlayerAirborneState(stateMachine));
// }
