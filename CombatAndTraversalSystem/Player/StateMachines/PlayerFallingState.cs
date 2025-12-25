namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerFallingState:PlayerBaseState
    {
        public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            //TODO crossfade falling anim hash
        }

        public override void Tick(float deltaTime)
        {
        }

        public override void Exit()
        {
        }
    }
}