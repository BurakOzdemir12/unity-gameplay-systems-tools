using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerFreeLookState : PlayerBaseState
    {
        public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.InputHandler.TargetEvent += OnTarget;
            stateMachine.InputHandler.DodgeEvent += OnDodge;
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.FreeLookBlendTreeHash,
                stateMachine.CrossFadeDurationBetweenBlendTrees);
        }


        public override void Tick(float deltaTime)
        {
            //If you want to attack while free look state even without Targeting use this code
            if (stateMachine.InputHandler.IsAttacking)
            {
                stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
                return;
            }

            // TrySwitchToBlockState(); If you want to block state by itself use blockstate
            HandleBlocking(deltaTime, allowBlocking: true);

            Vector3 movement = CalculateMovementDirection();

            Move(movement * stateMachine.FreeMovementSpeed, deltaTime);
            // stateMachine.Controller.Move(movement * stateMachine.FreeMovementSpeed * deltaTime); without force receiver -gravity

            if (stateMachine.InputHandler.Move.sqrMagnitude < 0.001f)
            {
                stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParam, 0,
                    stateMachine.LocomotionAnimatorDampTime, deltaTime);
                return;
            }

            stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParam, 1, stateMachine.LocomotionAnimatorDampTime,
                deltaTime);

            RotatePlayerTowardsMovement(movement, deltaTime);
        }


        public override void Exit()
        {
            stateMachine.InputHandler.TargetEvent -= OnTarget;
            stateMachine.InputHandler.DodgeEvent -= OnDodge;
        }

        private void RotatePlayerTowardsMovement(Vector3 movement, float deltaTime)
        {
            Quaternion targetRot = Quaternion.LookRotation(movement);

            Transform characterTransform = stateMachine.Controller.transform;
            characterTransform.rotation = Quaternion.Slerp(characterTransform.rotation, targetRot,
                stateMachine.RotationDampTime * deltaTime);

            // We can use RotateToward and Lerp either.
            // characterTransform.rotation = Quaternion.RotateTowards(
            //     characterTransform.rotation, targetRot, stateMachine.rotationDampTime * deltaTime
            // );
        }

        private void OnTarget()
        {
            if (!stateMachine.Targeter.SelectTarget()) return;

            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
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
    }
}