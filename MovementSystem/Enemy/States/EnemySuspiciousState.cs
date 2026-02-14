using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.PerceptionSystem;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemySuspiciousState : EnemyBaseState
    {
        private EnemyMovementDataSo data;
        private readonly Vector3 alertPosition;
        private EnemyPerceptionController perception;
        private float recognitionTimer;

        public EnemySuspiciousState(EnemyStateMachine stateMachine, Vector3 incomingPos) :
            base(stateMachine)
        {
            this.alertPosition = incomingPos;
        }

        public override void Enter()
        {
            recognitionTimer = 0f;
            data = stateMachine.EnemyConfigSo.MovementData;

            perception = stateMachine.EnemyPerceptionController;

            stateMachine.Agent.isStopped = false;
            stateMachine.Agent.speed = data.SuspiciousWalkSpeed;

            stateMachine.Animator.CrossFadeInFixedTime(data.SuspiciousBlendTreeHash, data.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            if (perception.CurrentTarget != null)
            {
                stateMachine.Agent.isStopped = true;
                stateMachine.Agent.velocity = Vector3.zero;

                RotateToPlayer(deltaTime);

                Vector3 targetPosition = perception.CurrentTarget.transform.position;
                float distanceToTarget = Vector3.Distance(stateMachine.transform.position, targetPosition);

                recognitionTimer += deltaTime;

                stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0f, data.LocomotionAnimatorDampTime,
                    deltaTime);

                if (recognitionTimer >= data.RecognitionTime || distanceToTarget <= data.InstantChaseDistance)
                {
                    stateMachine.SwitchState(new EnemyChaseState(stateMachine));
                    return;
                }
            }
            else
            {
                recognitionTimer = 0f;

                stateMachine.Agent.isStopped = false;

                MoveToDestination(destination: alertPosition, deltaTime);

                stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 1f, data.LocomotionAnimatorDampTime,
                    deltaTime);

                float distanceToSuspicious = Vector3.Distance(stateMachine.transform.position, alertPosition);
                if (distanceToSuspicious <= stateMachine.Agent.stoppingDistance || !perception.HasSuspiciousTarget)
                {
                    stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                    return;
                }
            }
        }

        public override void Exit()
        {
            stateMachine.Agent.ResetPath();
            stateMachine.Agent.isStopped = true;
            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0f);
        }

        private void MoveToDestination(Vector3 destination, float deltaTime)
        {
            if (stateMachine.Agent.isOnNavMesh)
            {
                stateMachine.Agent.SetDestination(destination);
            }

            Vector3 to = stateMachine.Agent.steeringTarget - stateMachine.transform.position;
            to.y = 0f;

            Vector3 dir = to.sqrMagnitude > 0.001f ? to.normalized : Vector3.zero;

            Move(dir * data.SuspiciousWalkSpeed, deltaTime);
            RotateToPlayer(deltaTime);

            stateMachine.Agent.nextPosition = stateMachine.transform.position;
        }
    }
}