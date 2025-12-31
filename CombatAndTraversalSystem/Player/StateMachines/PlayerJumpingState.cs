using _Project.Systems.ClimbingSystem.States.RootStates;
using _Project.Systems.CombatAndTraversalSystem.Player.Enums;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
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
            
            stateMachine.ForceReceiver.ApplyJumpForce(stateMachine.JumpForce);
            momentum = stateMachine.Controller.velocity;
            momentum.y = 0;

            stateMachine.Animator.CrossFadeInFixedTime(
                stateMachine.IdleToJumpHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            Move(momentum, deltaTime);

            if (stateMachine.Controller.velocity.y <= stateMachine.FallingVelocityThreshold)
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