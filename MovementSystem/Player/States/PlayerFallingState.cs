using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Player.States
{
    public class PlayerFallingState : PlayerBaseState
    {
        private PlayerAirborneState AirborneParent => GetSuperState() as PlayerAirborneState;
        private Vector3 momentum;

        public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            
            momentum = stateMachine.Controller.velocity;
            momentum.y = 0;
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.FallingLoopHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            Move(momentum, deltaTime);
            // TODO add climbing check when falling !maybe
        }

        public override void Exit()
        {
        }
    }
}