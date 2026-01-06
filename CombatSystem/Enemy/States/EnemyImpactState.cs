using _Project.Systems._Core.StateMachine.Enemy;
using _Project.Systems.MovementSystem.Enemy.States;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyImpactState : EnemyBaseState
    {
        public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        private float remainingImpactTime;

        public override void Enter()
        {
             remainingImpactTime = stateMachine.ImpactDuration;
            
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LightImpactHash,
                stateMachine.CrossFadeDurationCombat);
        }

        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

             remainingImpactTime -= deltaTime;

            if (remainingImpactTime <= 0)
            {
                if (IsInAttackRange())
                {
                    stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
                }
                else
                {
                    stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                }
            }
        }

        public override void Exit()
        {
        }
    }
}