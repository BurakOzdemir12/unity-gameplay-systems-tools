using _Project.Systems.ClimbingSystem.States.RootStates;
using _Project.Systems.CombatSystem.Player.States;
using _Project.Systems.GatheringSystem.Player.States;
using _Project.Systems.SharedGameplay.StateMachine.Player;
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
            stateMachine.InputHandler.RollOrJumpEvent += OnRollOrJump;
            stateMachine.InputHandler.InteractEvent += OnInteraction;
            stateMachine.InputHandler.DrawSheathEvent += OnDrawSheath;
            stateMachine.InputHandler.ParryEvent += OnParry;

            if (stateMachine.Targeter.SelectedTarget != null)
            {
                SetSubState(new PlayerTargetingState(stateMachine));
                return;
            }

            SetSubState(new PlayerFreeLookState(stateMachine));
        }


        public override void Tick(float deltaTime)
        {
            bool isValidState = stateMachine.CurrentSubState is PlayerFreeLookState or PlayerTargetingState;
            HandleCombatLayer(deltaTime, stateMachine.isWeaponEquipped && isValidState);

            if (stateMachine.IsGrounded)
            {
                ungroundedTime = 0f;
                return;
            }

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
            stateMachine.InputHandler.RollOrJumpEvent -= OnRollOrJump;
            stateMachine.InputHandler.InteractEvent -= OnInteraction;
            stateMachine.InputHandler.DrawSheathEvent -= OnDrawSheath;
            stateMachine.InputHandler.ParryEvent -= OnParry;


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

        private void OnRollOrJump()
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

        private void OnInteraction()
        {
            if (stateMachine.IsInAlertMode)
            {
                Debug.Log("Interaction not allowed in alert mode");
                return;
            }

            float cd = stateMachine.PlayerConfigSo.GatheringDataSet.cooldown;

            if (!stateMachine.CanUseInteractNow(cd)) return;

            if (stateMachine.CurrentSubState is PlayerGatheringState)
            {
                SetSubState(new PlayerFreeLookState(stateMachine));
                return;
            }

            bool isGatherOk = stateMachine.GatheringController.TryGatherAction();
            if (!isGatherOk) return;

            SetSubState(new PlayerGatheringState(stateMachine));
        }

        private void OnDrawSheath()
        {
            HandleSheathDraw();
        }

        private void OnParry()
        {
            if (!stateMachine.ShieldHandler.CurrentShieldLogic) return;

            SetSubState(new PlayerParryState(stateMachine));
        }
    }
}