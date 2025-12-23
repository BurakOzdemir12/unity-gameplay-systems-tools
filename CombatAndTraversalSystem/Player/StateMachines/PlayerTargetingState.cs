using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerTargetingState : PlayerBaseState
    {
        public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            stateMachine.InputHandler.TargetCancelEvent += OnTargetCancel;
            stateMachine.InputHandler.DodgeEvent += OnDodge;
            stateMachine.InputHandler.RollEvent += OnRoll;
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.TargetingBlendTreeHash,
                stateMachine.CrossFadeDurationBetweenBlendTrees);
        }


        public override void Tick(float deltaTime)
        {
            if (stateMachine.InputHandler.IsAttacking)
            {
                stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
                return;
            }

            if (stateMachine.Targeter.SelectedTarget == null)
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
                return;
            }

            HandleBlocking(deltaTime, true);

            Vector3 movement = CalculateMovement();
            Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);
            UpdateAnimator(deltaTime);
            FaceLockOnTarget(stateMachine.Targeter.SelectedTarget, deltaTime);
            Debug.Log(stateMachine.Targeter.SelectedTarget.name);
        }

        public override void Exit()
        {
            stateMachine.InputHandler.TargetCancelEvent -= OnTargetCancel;
            stateMachine.InputHandler.DodgeEvent -= OnDodge;
            stateMachine.InputHandler.RollEvent -= OnRoll;
        }

        private void OnTargetCancel()
        {
            stateMachine.Targeter.DeselectTarget();

            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }

        private void OnDodge()
        {
            if (Time.time - stateMachine.PreviousDodgeTime < stateMachine.DodgeCooldownTime)
            {
                return;
            }

            stateMachine.SetDodgeCooldownTime(Time.time);
            stateMachine.SwitchState(new PlayerDodgeState(stateMachine));
        }

        private void OnRoll()
        {
            if (Time.time - stateMachine.PreviousRollTime < stateMachine.RollCooldownTime)
            {
                return;
            }

            stateMachine.SetRollCooldownTime(Time.time);
            stateMachine.SwitchState(new PlayerRollState(stateMachine));
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