using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerHangingState : PlayerBaseState
    {
        private PlayerClimbState ClimbParent => GetSuperState() as PlayerClimbState;

        public PlayerHangingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        // HangingBracedHash
        // HangingFreeHandHash

        public override void Enter()
        {
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.BracedHangingHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            if (stateMachine.InputHandler.Move.y > 0)
            {
                ClimbParent?.SwitchSubState(new PlayerClimbUpState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}