using _Project.Systems._Core.GravityForce.Interfaces;
using _Project.Systems.HealthSystem.Health.Interfaces;
using _Project.Systems.HealthSystem.Structs;
using _Project.Systems.HealthSystem.Stun.Interfaces;
using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.SharedGameplay.BaseScriptableObjects.Characters;
using _Project.Systems.SharedGameplay.Shield_Logic.Structs;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyParryState : EnemyBaseState
    {
        public EnemyParryState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string PARRY_TAG = "Parry";
        private int layer;

        public override void Enter()
        {
            stateMachine.ShieldHandler.CurrentShieldLogic.ShieldParried += HandleParry;

            layer = stateMachine.BlockingLayerIndex;

            stateMachine.ShieldHandler.EnableShield();
            stateMachine.ShieldHandler.CurrentShieldLogic.SetParryWindow(true);

            stateMachine.Animator.SetLayerWeight(layer,
                1
            );

            EnemyConfigSo data = stateMachine.EnemyConfigSo;
            stateMachine.Animator.CrossFadeInFixedTime(data.CombatData.BlockParryAnimHash,
                data.CombatData.CrossFadeDurationCombat, layer);
        }


        public override void Tick(float deltaTime)
        {
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, layer, PARRY_TAG);

            if (normalizedTime >= 0.9f)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            }
        }

        public override void Exit()
        {
            stateMachine.ShieldHandler.CurrentShieldLogic.ShieldParried -= HandleParry;

            stateMachine.ShieldHandler.CurrentShieldLogic.SetParryWindow(false);
            stateMachine.ShieldHandler.DisableShield();
            stateMachine.Animator.SetLayerWeight(layer,
                0
            );
        }

        private void HandleParry(BlockContext ctx)
        {
            GameObject attackerGo = ctx.AttackerRoot.gameObject;

            DamageInfo damageInfo = new DamageInfo
            {
                Damage = stateMachine.ShieldHandler.CurrentShieldData.shieldDamage,
                TargetRoot = attackerGo,
                SourceObject = stateMachine.gameObject
            };

            var damageable = attackerGo.GetComponentInChildren<IDamageable>();
            damageable?.ApplyDamage(damageInfo);

            if (attackerGo.TryGetComponent<IKnockable>(out var knock))
            {
                Vector3 dir = (attackerGo.transform.position - stateMachine.transform.position);
                dir.y = 0f;
                knock.ApplyKnockback(stateMachine.ShieldHandler.CurrentShieldData.shieldKnockbackForce, dir.normalized);
            }

            var stunnable = attackerGo.GetComponent<IStunnable>() 
                            ?? attackerGo.GetComponentInChildren<IStunnable>();
            stunnable?.ApplyStun(stateMachine.ShieldHandler.CurrentShieldData.shieldStunPower);
        }
    }
}