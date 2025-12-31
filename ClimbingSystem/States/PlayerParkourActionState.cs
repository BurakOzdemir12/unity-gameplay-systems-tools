using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates;

namespace _Project.Systems.ClimbingSystem.States
{
    public class PlayerParkourActionState : PlayerBaseState
    {
        private string stepUpTag;
        private readonly ClimbTypeDataSo so;

        public PlayerParkourActionState(PlayerStateMachine stateMachine, ClimbTypeDataSo so) : base(stateMachine)
        {
            this.so = so;
        }

        public override void Enter()
        {
            stepUpTag = so.AnimTag;
            stateMachine.Animator.applyRootMotion = true;
            stateMachine.Controller.enabled = false;
            stateMachine.Animator.CrossFade(so.AnimHash, stateMachine.StepUpClimbCrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, stepUpTag);
            if (normalizedTime > 1f)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
            }
        }

        public override void Exit()
        {
            stateMachine.Animator.applyRootMotion = false;
            stateMachine.Controller.enabled = true;
        }
    }
}