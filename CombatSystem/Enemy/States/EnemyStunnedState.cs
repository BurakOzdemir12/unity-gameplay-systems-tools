using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyStunnedState : EnemyBaseState
    {
        private float stunTime;

        public EnemyStunnedState(EnemyStateMachine stateMachine, float duration) : base(stateMachine)
        {
            stunTime = duration;
        }

        public override void Enter()
        {
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.EnemyConfigSo.CombatData.StunnedAnimParamHash,
                stateMachine.EnemyConfigSo.CombatData.CrossFadeDurationCombat);
        }

        public override void Tick(float deltaTime)
        {
            stunTime -= deltaTime;
            if (stunTime <= 0)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}