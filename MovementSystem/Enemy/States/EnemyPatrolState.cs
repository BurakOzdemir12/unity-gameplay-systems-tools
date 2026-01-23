using _Project.Systems._Core.StateMachine.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemyPatrolState : EnemyBaseState
    {
        public EnemyPatrolState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }


        private float timer;

        public override void Enter()
        {
            timer = 0f;
            stateMachine.Agent.isStopped = false;

            stateMachine.Agent.speed = stateMachine.EnemyConfigSo.MovementData.FreeMovementSpeed;
            var roamingPos = stateMachine.firstSpawnPoint +
                             GetRandomDirection() *
                             Random.Range(2f, stateMachine.EnemyConfigSo.MovementData.patrolRange);

            stateMachine.Agent.SetDestination(roamingPos);
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LocomotionBlendTreeHash,
                stateMachine.EnemyConfigSo.MovementData.LocomotionBlendTreeDuration);
        }

        public override void Tick(float deltaTime)
        {
            timer += deltaTime;
            if (IsInChaseRange() || stateMachine.Agent.remainingDistance <= stateMachine.Agent.stoppingDistance)
            {
                stateMachine.SwitchState(new EnemyChaseState(stateMachine));
                return;
            }

            if (timer >= stateMachine.EnemyConfigSo.MovementData.patrolDuration)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }

            Patrolling(deltaTime);

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

        private void Patrolling(float deltaTime)
        {
            if (!stateMachine.Agent.isOnNavMesh || !stateMachine.Agent.enabled) return;

            Transform enemyT = stateMachine.transform;
            Vector3 to = stateMachine.Agent.steeringTarget - enemyT.position;
            to.y = 0f;

            Vector3 dir = to.sqrMagnitude > 0.001f ? to.normalized : Vector3.zero;
            Move(dir * stateMachine.EnemyConfigSo.MovementData.FreeMovementSpeed, deltaTime);

            RotateToPlayer(deltaTime);

            stateMachine.Agent.nextPosition = stateMachine.transform.position;
        }

        private Vector3 GetRandomDirection()
        {
            return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        }
    }
}