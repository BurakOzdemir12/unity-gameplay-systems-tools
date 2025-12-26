using _Project.Systems.CombatAndTraversalSystem.Player.Enums;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates
{
    public enum LandType
    {
        Hard,
        Soft
    }

    public class PlayerAirborneState : PlayerBaseState
    {
        private float startY;
        private float fallDistance;
        private bool fromJumping;

        public PlayerAirborneState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            startY = stateMachine.transform.position.y;
            fromJumping = stateMachine.ConsumeJump();

            if (fromJumping)
            {
                SetSubState(new PlayerJumpingState(stateMachine));
            }
            else
            {
                SetSubState(new PlayerFallingState(stateMachine));
            }
        }

        public override void Tick(float deltaTime)
        {
            Debug.Log(stateMachine.Controller.velocity.y);
            Move(deltaTime);
            if (!stateMachine.Controller.isGrounded) return;

            fallDistance = startY - stateMachine.transform.position.y;

            if (!fromJumping && fallDistance < stateMachine.LandingHeightThreshold)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
            }

            LandingType landingType = (fallDistance >= stateMachine.LandingHardHeightThreshold)
                ? LandingType.Heavy
                : LandingType.Light;

            if (subState is not PlayerLandingState)
                SetSubState(new PlayerLandingState(stateMachine, landingType));
        }

        public override void Exit()
        {
            ClearSubState();
        }

        public void SwitchSubState(PlayerBaseState newSubState) => SetSubState(newSubState);
    }
}