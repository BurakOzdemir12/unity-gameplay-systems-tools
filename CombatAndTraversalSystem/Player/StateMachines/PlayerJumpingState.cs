using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerJumpingState : PlayerBaseState
    {
        private Vector3 momentum;
        private PlayerAirborneState AirborneParent => GetSuperState() as PlayerAirborneState;

        public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.ForceReceiver.ApplyJumpForce(stateMachine.JumpForce);
            momentum = stateMachine.Controller.velocity;
            momentum.y = 0;
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.IdleToJumpHash, stateMachine.CrossFadeDuration);
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