using _Project.Systems._Core.StateMachine.Player;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerStunnedState : PlayerBaseState
    {
        private float stunTime;

        public PlayerStunnedState(PlayerStateMachine stateMachine, float duration) : base(stateMachine)
        {
            stunTime = duration;
        }

        public override void Enter()
        {
        }

        public override void Tick(float deltaTime)
        {
        }

        public override void Exit()
        {
        }
    }
}