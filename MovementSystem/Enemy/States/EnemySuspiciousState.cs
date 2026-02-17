using _Project.Systems.MovementSystem.Enemy.States;
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

            stateMachine.Agent.isStopped = true;
            stateMachine.Agent.speed = data.SuspiciousWalkSpeed;

            stateMachine.Animator.CrossFadeInFixedTime(data.SuspiciousBlendTreeHash, data.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            recognitionTimer += deltaTime;

            RotateToPlayer(deltaTime);

            //? time for shock time animation
            if (recognitionTimer <= 2f)
            {
                stateMachine.Agent.isStopped = true;
                stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0f,
                    data.LocomotionAnimatorDampTime, deltaTime);
                return;
            }

            //? Two Seconds later starts slowly walk to the alert position
            stateMachine.Agent.isStopped = false;
            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 1f,
                data.LocomotionAnimatorDampTime, deltaTime);

            MoveToDestination(alertPosition, deltaTime);

            //? If distance to alert position is less than stopping distance, switch to Idle state
            float distanceToAlertPos = Vector3.Distance(stateMachine.transform.position, alertPosition);
            if (distanceToAlertPos <= stateMachine.Agent.stoppingDistance)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }

            //? If recognition time is more than data.recognition time or distance too close to target
            //? and still have target onside, switch to chase state
            if (!perception.CurrentTarget) return;
            
            float distanceToTarget = Vector3.Distance(stateMachine.transform.position,
                perception.CurrentTarget.transform.position);
            if ((recognitionTimer >= data.RecognitionTime || distanceToTarget <= data.InstantChaseDistance) &&
                perception.CurrentTarget)
            {
                stateMachine.SwitchState(new EnemyChaseState(stateMachine));
                return;
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

            stateMachine.Agent.nextPosition = stateMachine.transform.position;
        }
    }
}