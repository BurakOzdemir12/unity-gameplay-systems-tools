namespace _Project.Systems.CombatAndTraversalSystem.Enemy.States
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