using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.CombatSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.MovementSystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Player.States
{
    public class PlayerFreeLookState : PlayerBaseState
    {
        private MovementDataSo data;

        public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private PlayerGroundedState GroundedParent => GetSuperState() as PlayerGroundedState;

        float targetSpeed;

        public override void Enter()
        {
            data = stateMachine.PlayerConfigSo.MovementData;

            stateMachine.InputHandler.TargetEvent += OnTarget;

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.FreeLookBlendTreeHash,
                data.CrossFadeDurationBetweenBlendTrees);
        }

        public override void Tick(float deltaTime)
        {
            //If you want to attack while free look state even without Targeting use this code
            if (stateMachine.InputHandler.IsAttacking)
            {
                GroundedParent?.SwitchSubState(new PlayerAttackingState(stateMachine, 0));
                // stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
                return;
            }


            // TrySwitchToBlockState(); If you want to block state by itself use blockstate
            HandleBlocking(deltaTime, allowBlocking: true);

            Vector3 movement = CalculateMovementDirection();
            bool isSprinting = stateMachine.InputHandler.IsSprinting;

            var finalSpeed = isSprinting ? data.FreeSprintSpeed : data.FreeMovementSpeed;

            Move(movement * finalSpeed, deltaTime);
            // stateMachine.Controller.Move(movement * stateMachine.FreeMovementSpeed * deltaTime); without force receiver -gravity

            if (stateMachine.InputHandler.Move.sqrMagnitude < 0.0001f)
            {
                stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParamHash, 0,
                    data.LocomotionAnimatorDampTime, deltaTime);
                return;
            }

            targetSpeed = isSprinting ? 1f : .5f;


            stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParamHash, targetSpeed,
                data.LocomotionAnimatorDampTime,
                deltaTime);

            RotatePlayerTowardsMovement(movement, deltaTime);
        }


        public override void Exit()
        {
            stateMachine.InputHandler.TargetEvent -= OnTarget;
            stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParamHash, 0);
        }

        private void RotatePlayerTowardsMovement(Vector3 movement, float deltaTime)
        {
            Quaternion targetRot = Quaternion.LookRotation(movement);

            Transform characterTransform = stateMachine.Controller.transform;
            characterTransform.rotation = Quaternion.Slerp(characterTransform.rotation, targetRot,
                data.RotationDampTime * deltaTime);

            // We can use RotateToward and Lerp either.
            // characterTransform.rotation = Quaternion.RotateTowards(
            //     characterTransform.rotation, targetRot, stateMachine.rotationDampTime * deltaTime
            // );
        }

        private void OnTarget()
        {
            if (!stateMachine.Targeter.SelectTarget()) return;
            GroundedParent?.SwitchSubState(new PlayerTargetingState(stateMachine));

            // stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        }
    }
}