using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyAttackCooldownState : EnemyBaseState
    {
        private float coolDownTimer = 0f;
        private EnemyCombatDataSo combatData;

        public EnemyAttackCooldownState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            combatData = stateMachine.EnemyConfigSo.CombatData;
            
            stateMachine.Animator.CrossFadeInFixedTime(combatData.CombatIdleAnimHash,
                combatData.CrossFadeDurationCombat);
            coolDownTimer = combatData.AttackCoolDown;
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