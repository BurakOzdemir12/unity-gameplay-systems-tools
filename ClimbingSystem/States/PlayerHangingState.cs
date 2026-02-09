using _Project.Systems.ClimbingSystem.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;

namespace _Project.Systems.ClimbingSystem.States
{
    public class PlayerHangingState : PlayerBaseState
    {
        private PlayerClimbState ClimbParent => GetSuperState() as PlayerClimbState;

        public PlayerHangingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }
            // public readonly int BracedHangingHash =  Animator.StringToHash("Braced Hanging Idle");
        // public readonly int BracedHangToCrouchClimbHash = Animator.StringToHash("Braced Hang To Crouch");
        // public readonly int FreeHangClimbHash = Animator.StringToHash("FreeHang Climb");
        // HangingBracedHash
        // HangingFreeHandHash

        public override void Enter()
        {
            // stateMachine.Animator.CrossFadeInFixedTime(stateMachine.BracedHangingHash,
            //     stateMachine.CrossFadeDuration);
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