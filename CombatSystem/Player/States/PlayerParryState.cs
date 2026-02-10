using _Project.Systems._Core.GravityForce.Interfaces;
using _Project.Systems.HealthSystem.Health.Interfaces;
using _Project.Systems.HealthSystem.Structs;
using _Project.Systems.HealthSystem.Stun.Interfaces;
using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.Shield_Logic.Structs;
using _Project.Systems.SharedGameplay.StateMachine.Player;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerParryState : PlayerBaseState
    {
        private PlayerGroundedState GroundParent => GetSuperState() as PlayerGroundedState;
        private int layer;
        private const string PARRY_TAG = "Parry";

        public PlayerParryState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            layer = stateMachine.BlockingLayerIndex;

            stateMachine.ShieldHandler.CurrentShieldLogic.ShieldParried += HandleParry;

            stateMachine.ShieldHandler.EnableShield();
            stateMachine.ShieldHandler.CurrentShieldLogic.SetParryWindow(true);

            stateMachine.Animator.SetLayerWeight(layer,
                1
            );

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.PlayerConfigSo.CombatData.BlockParryAnimHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, layer, PARRY_TAG);
            if (normalizedTime >= 0.9f)
            {
                GroundParent?.SwitchSubState(new PlayerFreeLookState(stateMachine));
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