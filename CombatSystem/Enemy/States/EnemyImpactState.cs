using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;

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
             remainingImpactTime = stateMachine.EnemyConfigSo.CombatData.ImpactDuration;
            
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LightImpactHash,
                stateMachine.EnemyConfigSo.CombatData.CrossFadeDurationCombat);
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