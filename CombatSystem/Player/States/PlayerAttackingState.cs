using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.CombatSystem.ScriptableObjects;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerAttackingState : PlayerBaseState
    {
        private readonly AttackDataSo attack;
        private float previousFrameTime;
        private bool alreadyAppliedForce = false;
        private const string ATTACK_TAG = "Attack";
        private PlayerGroundedState GroundParent => GetSuperState() as PlayerGroundedState;

        public PlayerAttackingState(PlayerStateMachine stateMachine, int attackIndex) : base(stateMachine)
        {
            attack = stateMachine.PlayerConfigSo.AttackTypeDataSet[attackIndex];
        }

        public override void Enter()
        {
            // stateMachine.WeaponLogic.SetAttackAttributes(attack.DamageMultiplier, attack.KnockBackForce,
            //     stateMachine.attackDamage);
            float finalDamage = stateMachine.PlayerConfigSo.CombatData.AttackDamage * attack.DamageMultiplier;
            float finalKnockbackForce = attack.KnockBackForce;

            stateMachine.WeaponLogic.BeginAttack(finalDamage, finalKnockbackForce);

            stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
        }

        public override void Tick(float deltaTime)
        {
            // Move(deltaTime, attack.AttackForceTargetPos, attack.AttackForce);
            Move(deltaTime);
            // FaceTarget(stateMachine.Targeter.SelectedTarget, deltaTime);

            float attackDampTime = stateMachine.PlayerConfigSo.CombatData.RotationDampTimeWhileAttack;

            RotateFaceToLook(deltaTime, attackDampTime);


            HandleBlocking(deltaTime, false);

            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, ATTACK_TAG);

            if (normalizedTime >= previousFrameTime && normalizedTime < 1f)
            {
                if (normalizedTime >= attack.ForceTime) //TODO Create Event action for this in scriptable object
                {
                    TryApplyForce();
                }

                if (stateMachine.InputHandler.IsAttacking)
                {
                    TryComboAttack(normalizedTime);
                }
            }
            else
            {
                if (stateMachine.Targeter.SelectedTarget != null)
                {
                    GroundParent?.SwitchSubState(new PlayerTargetingState(stateMachine));
                }
                else
                {
                    GroundParent?.SwitchSubState(new PlayerFreeLookState(stateMachine));
                }

                // stateMachine.DecideTargetOrLocomotion();
            }

            previousFrameTime = normalizedTime;
        }

        public override void Exit()
        {
            stateMachine.WeaponLogic.EndAttack();
        }

        private void TryComboAttack(float normalizedTime)
        {
            if (attack.ComboStateIndex == -1) return;
            if (normalizedTime < attack.ComboAttackTime) return;

            GroundParent?.SwitchSubState(new PlayerAttackingState(stateMachine, attack.ComboStateIndex));
        }

        private void TryApplyForce()
        {
            if (alreadyAppliedForce) return;
            stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * attack.AttackForce);
            alreadyAppliedForce = true;
        }
    }
}