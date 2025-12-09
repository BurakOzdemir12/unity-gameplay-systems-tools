using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerTestState : PlayerBaseState
    {
        public PlayerTestState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
        }

        public override void Tick(float deltaTime)
        {
            Vector3 movement = new Vector3();
            movement.x = stateMachine.InputHandler.Move.x;
            movement.y = 0;
            movement.z = stateMachine.InputHandler.Move.y;

            stateMachine.Controller.Move(movement * stateMachine.FreeMovementSpeed * deltaTime);

            if (stateMachine.InputHandler.Move.sqrMagnitude < 0.001f)
            {
                stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParam, 0, 0.1f, deltaTime);
                return;
            }

            stateMachine.Animator.SetFloat(stateMachine.FreeLookSpeedParam, 1, 0.1f, deltaTime);

            stateMachine.Controller.transform.rotation = Quaternion.LookRotation(movement);
        }

        public override void Exit()
        {
        }
    }
}