using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.PerceptionSystem;
using _Project.Systems.SharedGameplay.Managers.Effects.Audio;
using _Project.Systems.SharedGameplay.Managers.Effects.Audio.Enums;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemySuspiciousState : EnemyBaseState
    {
        private EnemyMovementDataSo data;
        private Vector3 alertPosition;
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

            if (data.SuspiciousAudioClips != null)
            {
                var toPlay = data.SuspiciousAudioClips[Random.Range(0, data.SuspiciousAudioClips.Length)];
                SoundManager.Instance.PlayGeneric3DSound(toPlay, stateMachine.transform.position, SoundChannel.CombatVocal,
                    data.VoiceVolume,
                    true, false);
            }

            stateMachine.Agent.isStopped = true;
            stateMachine.Agent.speed = data.SuspiciousWalkSpeed;

            stateMachine.Animator.CrossFadeInFixedTime(data.SuspiciousBlendTreeHash, data.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            //? If the target position changed, and it's not the player, reset the path, stop movement and change the new alert position
            HandleTargetPositionChange();

            //! If the alert is Player, then update the alert position to the player position, because it looks like stupid
            if (perception.CurrentTarget)
            {
                alertPosition = perception.CurrentTarget.transform.position;
            }

            recognitionTimer += deltaTime;
            RotateToPlayer(deltaTime);

            //? If recognition time is more than data.recognition time or distance too close to target
            //? and still have target onside, switch to chase state
            if (CheckAndSwitchToChase()) return;

            //? If the recognition phase is active, stop movement and play Shock Anim.
            if (IsRecognitionPhaseActive())
            {
                stateMachine.Agent.isStopped = true;
                stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0f,
                    data.LocomotionAnimatorDampTime, deltaTime);
                return;
            }


            //? Two Seconds later starts slowly walk to the alert position
            StartMovingToAlertPosition(deltaTime);

            //? If distance to alert position is less than stopping distance, switch to Idle state
            if (HasReachedAlertPosition()) return;
        }

        private bool HasReachedAlertPosition()
        {
            float distanceToAlertPos = Vector3.Distance(stateMachine.transform.position, alertPosition);
            if (distanceToAlertPos <= stateMachine.Agent.stoppingDistance)
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return true;
            }

            return false;
        }

        private void StartMovingToAlertPosition(float deltaTime)
        {
            stateMachine.Agent.isStopped = false;
            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 1f,
                data.LocomotionAnimatorDampTime, deltaTime);

            MoveToDestination(alertPosition, deltaTime);
        }

        private bool IsRecognitionPhaseActive() => recognitionTimer <= data.RecognitionTime;

        private bool CheckAndSwitchToChase()
        {
            if (!perception.CurrentTarget) return false;

            float distanceToTarget = Vector3.Distance(stateMachine.transform.position,
                perception.CurrentTarget.transform.position);
            if ((recognitionTimer >= data.RecognitionTime || distanceToTarget <= data.InstantChaseDistance) &&
                perception.CurrentTarget)
            {
                stateMachine.SwitchState(new EnemyChaseState(stateMachine));
                return true;
            }

            return false;
        }

        private void HandleTargetPositionChange()
        {
            float distanceNewOldPos = Vector3.Distance(alertPosition, perception.LastKnownTargetPos);
            if (distanceNewOldPos > 1f && !perception.CurrentTarget)
            {
                alertPosition = perception.LastKnownTargetPos;
                recognitionTimer = 0f;

                stateMachine.Agent.isStopped = true;
                stateMachine.Agent.ResetPath();
            }
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

        public override void Exit()
        {
            stateMachine.Agent.ResetPath();
            stateMachine.Agent.isStopped = true;
            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0f);
        }
    }
}