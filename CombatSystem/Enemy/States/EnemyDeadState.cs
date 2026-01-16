using _Project.Systems._Core.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyDeadState : EnemyBaseState
    {
        public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.Ragdoll.ToggleRagdoll(true);
            stateMachine.WeaponHandler.CurrentWeaponLogic.gameObject.SetActive(false);
            Object.Destroy(stateMachine.Target);
        }

        public override void Tick(float deltaTime)
        {
        }

        public override void Exit()
        {
        }
    }
}