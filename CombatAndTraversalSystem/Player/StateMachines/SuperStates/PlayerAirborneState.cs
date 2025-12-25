namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates
{
    public class PlayerAirborneState : PlayerBaseState
    {
        public PlayerAirborneState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            //setsubsatet falling
            
        }

        public override void Tick(float deltaTime)
        {
            if (stateMachine.Controller.isGrounded)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
            }
            
            Move(deltaTime);
        }

        public override void Exit()
        {
        }
    }
}