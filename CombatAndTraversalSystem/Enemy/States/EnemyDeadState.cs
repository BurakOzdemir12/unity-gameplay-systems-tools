using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Enemy.States
{
    public class EnemyDeadState : EnemyBaseState
    {
        public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            
            
            
            stateMachine.Ragdoll.ToggleRagdoll(true);
            stateMachine.WeaponLogic.gameObject.SetActive(false);
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