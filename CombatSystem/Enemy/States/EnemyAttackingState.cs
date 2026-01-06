using _Project.Systems._Core.StateMachine.Enemy;
using _Project.Systems.MovementSystem.Enemy.States;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyAttackingState : EnemyBaseState
    {
        public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string ATTACK_TAG = "Attack";

        public override void Enter()
        {
            // if you want to use a blend tree for attacks, use blend tree drawing arrow sword by for distance
            // stateMachine.Animator.CrossFadeInFixedTime(stateMachine.AttackBlendTreeHash,
            //     stateMachine.CrossFadeDurationCombat); 

            float finalDamage = stateMachine.EnemyConfigSo.CombatData.AttackDamage;
            float finalKnockbackForce = stateMachine.EnemyConfigSo.CombatData.AttackKnockBackForce;

            stateMachine.WeaponLogic.BeginAttack(finalDamage, finalKnockbackForce);

            // stateMachine.WeaponLogic.SetAttackAttributes(1f, stateMachine.AttackKnockBackForce,
            //     stateMachine.AttackDamage);

            stateMachine.Animator.SetFloat(stateMachine.MoveSpeedParamHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.EnemyAttack1RHash,
                stateMachine.EnemyConfigSo.CombatData.CrossFadeDurationCombat);
        }


        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            
            RotateToPlayer(deltaTime);
            //TODO Create State Machine behaviour script (EnemyAttackStateBehaviour ) for changes between states.


            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0,ATTACK_TAG);
            if (normalizedTime >= 1f)
            {
                stateMachine.SwitchState(IsInAttackRange()
                    ? new EnemyAttackCooldownState(stateMachine)
                    : new EnemyChaseState(stateMachine));
            }
        }

        public override void Exit()
        {
            stateMachine.WeaponLogic.EndAttack();
        }
    }
}