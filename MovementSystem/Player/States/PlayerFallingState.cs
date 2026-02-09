using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.SharedGameplay.StateMachine.Player;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Player.States
{
    public class PlayerFallingState : PlayerBaseState
    {
        private PlayerAirborneState AirborneParent => GetSuperState() as PlayerAirborneState;
        private Vector3 momentum;
        private FallLandDataSo data;
        public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            data = stateMachine.PlayerConfigSo.FallLandData;
            momentum = stateMachine.Controller.velocity;
            momentum.y = 0;
            stateMachine.Animator.CrossFadeInFixedTime(data.FallingLoopHash, stateMachine.CrossFadeDuration);
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