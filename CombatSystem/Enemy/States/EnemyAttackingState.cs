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
        private EnemyMovementDataSo data;

        public override void Enter()
        {
            // if you want to use a blend tree for attacks, use blend tree drawing arrow sword by for distance
            // stateMachine.Animator.CrossFadeInFixedTime(stateMachine.AttackBlendTreeHash,
            //     stateMachine.CrossFadeDurationCombat); 
            data = stateMachine.EnemyConfigSo.MovementData;
            float finalDamage = stateMachine.EnemyConfigSo.CombatData.AttackDamage;
            float finalKnockbackForce = stateMachine.EnemyConfigSo.CombatData.AttackKnockBackForce;

            stateMachine.WeaponHandler.CurrentWeaponLogic.SetupAttack(finalDamage, finalKnockbackForce, "normal");

            // stateMachine.WeaponLogic.SetAttackAttributes(1f, stateMachine.AttackKnockBackForce,
            //     stateMachine.AttackDamage);

            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.EnemyAttack1RHash,
                stateMachine.EnemyConfigSo.CombatData.CrossFadeDurationCombat);
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