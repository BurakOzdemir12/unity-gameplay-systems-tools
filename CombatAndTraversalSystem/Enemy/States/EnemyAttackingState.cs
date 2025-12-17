namespace _Project.Systems.CombatAndTraversalSystem.Enemy.States
{
    public class EnemyAttackingState : EnemyBaseState
    {
        public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            // stateMachine.Animator.CrossFadeInFixedTime(stateMachine.AttackBlendTreeHash,
            //     stateMachine.CrossFadeDurationCombat);
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.EnemyAttack1RHash,
                stateMachine.CrossFadeDurationCombat);
        }

        //TODO create Combat Attack cooldown
        
        public override void Tick(float deltaTime)
        {
            stateMachine.Animator.SetFloat(stateMachine.MoveSpeedParamHash, 0f,
                stateMachine.locomotionAnimationDampTime,
                deltaTime);


            if (!IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }

            RotateToPlayer(deltaTime);
        }

        public override void Exit()
        {
        }
    }
}