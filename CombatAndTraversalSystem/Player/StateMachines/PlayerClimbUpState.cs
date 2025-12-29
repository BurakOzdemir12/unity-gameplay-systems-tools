using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerClimbUpState : PlayerBaseState
    {
        private const string CLIMB_UP_TAG = "ClimbUp";

        public PlayerClimbUpState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        // BracedHangToCrouchClimbHash
        //     FreeHangClimbHash
        public override void Enter()
        {
            if (stateMachine.IsFreeFlowClimb)
            {
                stateMachine.Animator.CrossFadeInFixedTime(stateMachine.BracedHangToCrouchClimbHash,
                    stateMachine.CrossFadeDuration);
                return;
            }

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.FreeHangClimbHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            // var tag =
                float normalisedTime = GetNormalizedTime(stateMachine.Animator, 0, CLIMB_UP_TAG);
        }

        public override void Exit()
        {
        }
    }
}