using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyAttackCooldownState : EnemyBaseState
    {
        private float coolDownTimer = 0f;

        public EnemyAttackCooldownState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.CombatIdleHash,
                stateMachine.EnemyConfigSo.CombatData.CrossFadeDurationCombat);
            coolDownTimer = stateMachine.EnemyConfigSo.CombatData.AttackCoolDown;
        }

        public override void Tick(float deltaTime)
        {
            HandleBlocking(deltaTime, true);
            
            if (!IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }

            RotateToPlayer(deltaTime);

            coolDownTimer -= deltaTime;
            if (coolDownTimer <= 0f)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }
        }

        public override void Exit()
        {
        }
    }
}