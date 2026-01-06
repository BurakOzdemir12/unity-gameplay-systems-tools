using _Project.Systems._Core.StateMachine.Enemy;
using _Project.Systems.MovementSystem.Enemy.States;

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
                stateMachine.CrossFadeDurationCombat);
            coolDownTimer = stateMachine.AttackCoolDown;
        }

        public override void Tick(float deltaTime)
        {
            if (!IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }

            RotateToPlayer(deltaTime);

            coolDownTimer -= deltaTime;
            if (coolDownTimer <= 0f)
            {
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
                return;
            }
        }

        public override void Exit()
        {
        }
    }
}