using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerFreeLookState : PlayerBaseState
    {
        public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.InputHandler.TargetEvent += OnTarget;
        }

        private void OnTarget()
        {
            if (!stateMachine.Targeter.SelectTarget()) return;

            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        }

        public override void Tick(float deltaTime)
        {
            Vector3 movement = CalculateMovement();

            stateMachine.Controller.Move(movement * stateMachine.FreeMovementSpeed * deltaTime);

            if (stateMachine.InputHandler.Move.sqrMagnitude < 0.001f)
            {
                stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParam, 0,
                    stateMachine.locomotionAnimatorDampTime, deltaTime);
                return;
            }

            stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParam, 1, stateMachine.locomotionAnimatorDampTime,
                deltaTime);

            RotatePlayerTowardsMovement(movement, deltaTime);
        }


        public override void Exit()
        {
            stateMachine.InputHandler.TargetEvent -= OnTarget;
        }

        private void RotatePlayerTowardsMovement(Vector3 movement, float deltaTime)
        {
            Quaternion targetRot = Quaternion.LookRotation(movement);

            Transform characterTransform = stateMachine.Controller.transform;
            characterTransform.rotation = Quaternion.Slerp(characterTransform.rotation, targetRot,
                stateMachine.rotationDampTime * deltaTime);

            // We can use RotateToward and Lerp either.
            // characterTransform.rotation = Quaternion.RotateTowards(
            //     characterTransform.rotation, targetRot, stateMachine.rotationDampTime * deltaTime
            // );
        }

        private Vector3 CalculateMovement()
        {
            Vector3 forward = stateMachine.MainCameraTransform.forward;
            Vector3 right = stateMachine.MainCameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return forward * stateMachine.InputHandler.Move.y +
                   right * stateMachine.InputHandler.Move.x;
        }
    }
}