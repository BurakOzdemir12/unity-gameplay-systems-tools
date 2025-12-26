using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
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
        }

        public override void Exit()
        {
        }
    }
}