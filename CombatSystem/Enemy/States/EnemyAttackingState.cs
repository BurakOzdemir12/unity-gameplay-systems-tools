using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyAttackingState : EnemyBaseState
    {
        public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string ATTACK_TAG = "Attack";
        private EnemyMovementDataSo movementData;
        private EnemyCombatDataSo combatData;

        public override void Enter()
        {
            movementData = stateMachine.EnemyConfigSo.MovementData;
            combatData = stateMachine.EnemyConfigSo.CombatData;

            stateMachine.EnemyPerceptionController.IsAggressive = true;

            float finalDamage = combatData.AttackDamage;
            float finalKnockbackForce = combatData.AttackKnockBackForce;

            stateMachine.WeaponHandler.CurrentWeaponLogic.SetupAttack(finalDamage, finalKnockbackForce, "normal");

            stateMachine.Animator.SetFloat(movementData.FreeLookSpeedParamHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(combatData.EnemyAttack1RHash,
                combatData.CrossFadeDurationCombat);
        }


        public override void Tick(float deltaTime)
        {
            Move(deltaTime);
            RotateToPlayer(deltaTime);

            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, ATTACK_TAG);
            if (normalizedTime >= 0.9f)
            {
                stateMachine.SwitchState(new EnemyAttackCooldownState(stateMachine));
            }
        }

        public override void Exit()
        {
            stateMachine.WeaponHandler.CurrentWeaponLogic.EndAttack();
        }
    }
}