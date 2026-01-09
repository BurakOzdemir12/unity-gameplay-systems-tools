using _Project.Systems._Core.StateMachine.Enemy;
using _Project.Systems.CombatSystem.Enemy.States;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.Agent.isStopped = false;

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LocomotionBlendTreeHash,
                stateMachine.EnemyConfigSo.MovementData.LocomotionBlendTreeDuration);
        }

        public override void Tick(float deltaTime)
        {
            if (!IsInChaseRange())
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }

            if (IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
                return;
            }

            MoveToPlayer(deltaTime);
            stateMachine.Animator.SetFloat(stateMachine.MoveSpeedParamHash, 1f,
                stateMachine.EnemyConfigSo.MovementData.LocomotionAnimatorDampTime,
                deltaTime);
        }

        public override void Exit()
        {
            stateMachine.Agent.ResetPath();
            stateMachine.Agent.isStopped = true;
            stateMachine.Animator.SetFloat(stateMachine.MoveSpeedParamHash, 0);
        }

        private void MoveToPlayer(float deltaTime)
        {
            if (stateMachine.Player == null) return;
            // bool isArrived = stateMachine.Agent.remainingDistance <= stateMachine.Agent.stoppingDistance;
            if (stateMachine.Agent.isOnNavMesh) //!isArrived &&
            {
                Vector3 detectedPlayerPos = stateMachine.Player.transform.position;
                stateMachine.Agent.SetDestination(detectedPlayerPos);
            }

            Transform enemyT = stateMachine.transform;
            Vector3 to = stateMachine.Agent.steeringTarget - enemyT.position;
            to.y = 0f;

            Vector3 dir = to.sqrMagnitude > 0.001f ? to.normalized : Vector3.zero;

            Move(dir * stateMachine.EnemyConfigSo.MovementData.FreeMovementSpeed, deltaTime);

            RotateToPlayer(deltaTime);
            // stateMachine.Agent.velocity = stateMachine.Controller.velocity;
            stateMachine.Agent.nextPosition = stateMachine.transform.position;
        }
    }
}