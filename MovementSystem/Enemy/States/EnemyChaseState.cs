using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.SharedGameplay.Managers.Effects.Audio;
using _Project.Systems.SharedGameplay.Managers.Effects.Audio.Enums;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemyChaseState : EnemyBaseState
    {
        private EnemyMovementDataSo data;
        private float stoppingDistance;
        private float timer;

        public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            timer = 0f;
            data = stateMachine.EnemyConfigSo.MovementData;

            stateMachine.EnemyPerceptionController.IsAggressive = true;

            if (data.AlertAudioClips != null)
            {
                AudioClip toPlay = data.AlertAudioClips[Random.Range(0, data.AlertAudioClips.Length)];
                SoundManager.Instance.PlayGeneric3DSound(toPlay, stateMachine.transform.position,
                    SoundChannel.CombatVocal, data.VoiceVolume, true, false);
            }

            stateMachine.Agent.isStopped = false;
            stateMachine.Agent.speed = data.FreeMovementSpeed;
            stoppingDistance = stateMachine.Agent.stoppingDistance;

            stateMachine.Animator.CrossFadeInFixedTime(data.LocomotionBlendTreeHash,
                data.LocomotionBlendTreeDuration);
        }

        public override void Tick(float deltaTime)
        {
            var perception = stateMachine.EnemyPerceptionController;

            if (perception.IsTargetInAttackRange)
            {
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
                return;
            }

            Vector3 destination = perception.LastKnownTargetPos;
            float distanceToTarget = Vector3.Distance(stateMachine.transform.position, destination);

            bool isTargetVisible = perception.IsTargetVisible(perception.CurrentTarget);

            if (isTargetVisible)
            {
                timer = 0f;
            }
            else
            {
                timer += deltaTime;
                bool reachedLastKnownPos = distanceToTarget <= Mathf.Max(stoppingDistance, 1.5f);
                bool timeIsUp = timer > data.TargetPersistenceMemory;

                if (reachedLastKnownPos || timeIsUp)
                {
                    Debug.Log("I Lost the enemy but i will follow the last pos.");
                    stateMachine.SwitchState(new EnemySuspiciousState(stateMachine, destination));
                    return;
                }
            }

            MoveToPlayer(destination, deltaTime);

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

        private void MoveToPlayer(Vector3 destination, float deltaTime)
        {
            if (stateMachine.Agent.isOnNavMesh)
            {
                stateMachine.Agent.SetDestination(destination);
            }

            Transform enemyT = stateMachine.transform;
            Vector3 to = stateMachine.Agent.steeringTarget - enemyT.position;
            to.y = 0f;
            Vector3 dir = to.sqrMagnitude > 0.001f ? to.normalized : Vector3.zero;

            Move(dir * data.FreeMovementSpeed, deltaTime);
            RotateToPlayer(deltaTime);
            stateMachine.Agent.nextPosition = stateMachine.transform.position;
        }
    }
}