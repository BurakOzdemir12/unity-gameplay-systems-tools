using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.SharedGameplay.StateMachine.Player;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerTargetingState : PlayerBaseState
    {
        private MovementDataSo data;

        public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private PlayerGroundedState GroundedParent => GetSuperState() as PlayerGroundedState;

        public override void Enter()
        {
            stateMachine.IsInAlertMode = true;
            data = stateMachine.PlayerConfigSo.MovementData;
            stateMachine.InputHandler.TargetCancelEvent += OnTargetCancel;
            stateMachine.Animator.CrossFadeInFixedTime(data.TargetingBlendTreeHash,
                data.CrossFadeDurationBetweenBlendTrees);
        }


        public override void Tick(float deltaTime)
        {
            if (stateMachine.InputHandler.IsAttacking)
            {
                GroundedParent?.SwitchSubState(new PlayerAttackingState(stateMachine, 0));
                // stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
                return;
            }

            if (stateMachine.Targeter.SelectedTarget == null)
            {
                GroundedParent?.SwitchSubState(new PlayerFreeLookState(stateMachine));
                // stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
                return;
            }

            HandleBlocking(deltaTime, true);

            Vector3 movement = CalculateMovement();
            Move(movement * data.TargetingMovementSpeed, deltaTime);
            UpdateAnimator(deltaTime);
            FaceLockOnTarget(stateMachine.Targeter.SelectedTarget, deltaTime);
        }

        public override void Exit()
        {
            stateMachine.IsInAlertMode = false;
            stateMachine.InputHandler.TargetCancelEvent -= OnTargetCancel;
        }

        private void OnTargetCancel()
        {
            stateMachine.Animator.SetFloat(data.TargetingForwardSpeedHash, 0);
            stateMachine.Animator.SetFloat(data.TargetingRightSpeedHash, 0);

            stateMachine.Targeter.DeselectTarget();
            GroundedParent?.SwitchSubState(new PlayerFreeLookState(stateMachine));
            // stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }


        private Vector3 CalculateMovement()
        {
            Vector3 movement = new Vector3();
            movement += stateMachine.transform.right * stateMachine.InputHandler.Move.x;
            movement += stateMachine.transform.forward * stateMachine.InputHandler.Move.y;

            return movement;
        }

        private void UpdateAnimator(float deltaTime)
        {
            Vector3 movement = stateMachine.InputHandler.Move;
            if (movement.y == 0)
            {
                stateMachine.Animator.SetFloat(data.TargetingForwardSpeedHash, 0,
                    data.TargetingAnimatorDampTime, deltaTime);
            }
            else
            {
                float value = movement.y > 0f ? 1f : -1f;
                stateMachine.Animator.SetFloat(data.TargetingForwardSpeedHash, value,
                    data.TargetingAnimatorDampTime, deltaTime);
            }

            if (movement.x == 0)
            {
                stateMachine.Animator.SetFloat(data.TargetingRightSpeedHash, 0,
                    data.TargetingAnimatorDampTime, deltaTime);
            }
            else
            {
                float value = movement.x > 0 ? 1f : -1f;
                stateMachine.Animator.SetFloat(data.TargetingRightSpeedHash, value,
                    data.TargetingAnimatorDampTime, deltaTime);
            }

            if (movement.sqrMagnitude < 0.001f)
            {
                stateMachine.Animator.SetFloat(data.TargetingForwardSpeedHash, 0);
                stateMachine.Animator.SetFloat(data.TargetingRightSpeedHash, 0);
            }
        }
    }
}