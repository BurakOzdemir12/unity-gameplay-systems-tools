using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemyChaseState : EnemyBaseState
    {
        private EnemyMovementDataSo data;
        
        public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            data = stateMachine.EnemyConfigSo.MovementData;
            
            stateMachine.Agent.isStopped = false;
            stateMachine.Agent.speed = data.FreeMovementSpeed;

            stateMachine.Animator.CrossFadeInFixedTime(data.LocomotionBlendTreeHash,
                data.LocomotionBlendTreeDuration);
        }

        public override void Tick(float deltaTime)
        {
            if (!IsInChaseRange() )
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
            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 1f,
                data.LocomotionAnimatorDampTime,
                deltaTime);

            HandleBlocking(deltaTime, true);
        }

        public override void Exit()
        {
            stateMachine.Agent.ResetPath();
            stateMachine.Agent.isStopped = true;
            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0);
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

            Move(dir * data.FreeMovementSpeed, deltaTime);

            RotateToPlayer(deltaTime);
            // stateMachine.Agent.velocity = stateMachine.Controller.velocity;
            stateMachine.Agent.nextPosition = stateMachine.transform.position;

        }
    }
}