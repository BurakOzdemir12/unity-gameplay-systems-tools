using Unity.AppUI.Core;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerImpactState : PlayerBaseState
    {
        public PlayerImpactState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private float remainingImpactTime;

        public override void Enter()
        {
            remainingImpactTime = stateMachine.ImpactDuration;

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LightImpactHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            remainingImpactTime -= deltaTime;

            if (remainingImpactTime <= 0f)
            {
                DecideTargetOrLocomotion();
            }
        }

        public override void Exit()
        {
        }
    }
}