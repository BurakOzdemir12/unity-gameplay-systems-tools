using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Enemy.States
{
    public class EnemyImpactState : EnemyBaseState
    {
        private float remainingImpactTime;
        private EnemyCombatDataSo combatData;

        public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            combatData = stateMachine.EnemyConfigSo.CombatData;

            remainingImpactTime = combatData.ImpactDuration;

            stateMachine.Animator.CrossFadeInFixedTime(combatData.ImpactSlightAnimHash,
                combatData.CrossFadeDurationCombat);

            stateMachine.EnemyPerceptionController.IsAggressive = true;
        }

        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            remainingImpactTime -= deltaTime;

            if (remainingImpactTime <= 0)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}