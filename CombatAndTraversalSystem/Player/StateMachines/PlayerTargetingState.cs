using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerTargetingState : PlayerBaseState
    {
        public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private PlayerGroundedState GroundedParent => GetSuperState() as PlayerGroundedState;

        public override void Enter()
        {
            stateMachine.InputHandler.TargetCancelEvent += OnTargetCancel;
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.TargetingBlendTreeHash,
                stateMachine.CrossFadeDurationBetweenBlendTrees);
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
            Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);
            UpdateAnimator(deltaTime);
            FaceLockOnTarget(stateMachine.Targeter.SelectedTarget, deltaTime);
        }

        public override void Exit()
        {
            stateMachine.InputHandler.TargetCancelEvent -= OnTargetCancel;
        }

        private void OnTargetCancel()
        {
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
                stateMachine.Animator.SetFloat(stateMachine.TargetingForwardSpeedHash, 0,
                    stateMachine.TargetingAnimatorDampTime, deltaTime);
            }
            else
            {
                float value = movement.y > 0f ? 1f : -1f;
                stateMachine.Animator.SetFloat(stateMachine.TargetingForwardSpeedHash, value,
                    stateMachine.TargetingAnimatorDampTime, deltaTime);
            }

            if (movement.x == 0)
            {
                stateMachine.Animator.SetFloat(stateMachine.TargetingRightSpeedHash, 0,
                    stateMachine.TargetingAnimatorDampTime, deltaTime);
            }
            else
            {
                float value = movement.x > 0 ? 1f : -1f;
                stateMachine.Animator.SetFloat(stateMachine.TargetingRightSpeedHash, value,
                    stateMachine.TargetingAnimatorDampTime, deltaTime);
            }

            if (movement.sqrMagnitude < 0.001f)
            {
                stateMachine.Animator.SetFloat(stateMachine.TargetingForwardSpeedHash, 0);
                stateMachine.Animator.SetFloat(stateMachine.TargetingRightSpeedHash, 0);
            }
        }
    }
}