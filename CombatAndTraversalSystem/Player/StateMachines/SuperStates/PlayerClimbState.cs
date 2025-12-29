using _Project.Systems.CombatAndTraversalSystem.Player.Enums;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates
{
    public class PlayerClimbState : PlayerBaseState
    {
        private float detectedHeight;

        public PlayerClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            // if detected height is less than Auto Climb height, climb up, else goes Hang State 
            detectedHeight = stateMachine.LedgeValidator.lockedLedge.height;
            // wallNormal = stateMachine.LedgeValidator.lockedLedge.wallNormal;
            // surfacePoint = stateMachine.LedgeValidator.lockedLedge.surfacePoint;
            // detectedHeight <= stateMachine.AutoClimbMaxHeight
            if (stateMachine.IsFreeFlowClimb)
            {
                SetSubState(new PlayerHangingState(stateMachine));
            }
            else
            {
                SetSubState(new PlayerClimbUpState(stateMachine));
            }
        }

        public override void Tick(float deltaTime)
        {
        }

        public override void Exit()
        {
            stateMachine.LedgeValidator.ClearLock();
            ClearSubState();
        }

        public void SwitchSubState(PlayerBaseState newSubState) => SetSubState(newSubState);
    }
}