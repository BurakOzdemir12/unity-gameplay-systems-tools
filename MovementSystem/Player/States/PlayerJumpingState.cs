using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.MovementSystem.Player.Enums;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Player.States
{
    public class PlayerJumpingState : PlayerBaseState
    {
        private Vector3 momentum;
        private JumpVariant jumpType;

        private PlayerAirborneState AirborneParent => GetSuperState() as PlayerAirborneState;
        private bool edgeFound;
        private const string JUMP_TAG = "Jump";
        private const string CLIMB_JUMP_TAG = "JumpClimb";
        private float normalisedTime;

        public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            // jumpType = JumpVariant.Normal;

            stateMachine.ForceReceiver.ApplyJumpForce(stateMachine.PlayerConfigSo.JumpData.JumpForce);
            momentum = stateMachine.Controller.velocity;
            momentum.y = 0;

            stateMachine.Animator.CrossFadeInFixedTime(
                stateMachine.IdleToJumpHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            Move(momentum, deltaTime);

            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, JUMP_TAG);
            if (stateMachine.GroundChecker.DistanceToGround >= stateMachine.PlayerConfigSo.FallLandData.FallingHeightThreshold &&
                normalizedTime >= 0.4f)
            {
                AirborneParent?.SwitchSubState(new PlayerFallingState(stateMachine));
                return;
            }
        }

        public override void Exit()
        {
        }
    }
}