using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems._Core.StateMachine.Enemy
{
    public abstract class EnemyBaseState : State
    {
        protected readonly EnemyStateMachine stateMachine;

        protected EnemyBaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        // protected bool IsInChaseRange()
        // {
        //     int detectedCount = Physics.OverlapSphereNonAlloc(
        //         stateMachine.transform.position,
        //         stateMachine.EnemyConfigSo.MovementData.ChaseDetectionRange,
        //         stateMachine.buffersForChase,
        //         stateMachine.EnemyConfigSo.MovementData.ChaseDetectionLayers,
        //         QueryTriggerInteraction.Ignore
        //     );
        //
        //     if (detectedCount == 0)
        //     {
        //         stateMachine.Player = null;
        //         return false;
        //     }
        //
        //     Collider[] colType = stateMachine.buffersForChase;
        //     GameObject closestTarget = FindClosestTarget(detectedCount, colType);
        //
        //
        //     return closestTarget != null;
        // }

        protected bool IsInAttackRange()
        {
            int detected = Physics.OverlapSphereNonAlloc(
                stateMachine.transform.position + stateMachine.EnemyConfigSo.CombatData.AttackPosition,
                stateMachine.EnemyConfigSo.CombatData.AttackRange,
                stateMachine.buffersForAttack,
                stateMachine.EnemyConfigSo.CombatData.AttackDetectionLayers,
                QueryTriggerInteraction.Ignore);
            if (detected == 0)
                
            {
                return false;
            }

            Collider[] colType = stateMachine.buffersForAttack;

            GameObject closestTarget = FindClosestTarget(detected, colType);

            return closestTarget != null;
        }

        protected bool IsInChaseRange()
        {
            var targets = stateMachine.FieldOfView.Targets;
            if (targets == null || targets.Count == 0)
            {
                stateMachine.Player = null;
                return false;
            }

            GameObject closestTarget = FindClosestTargetFromTargets(targets);
            stateMachine.Player = closestTarget;

            return closestTarget != null;
        }

        private GameObject FindClosestTargetFromTargets(List<GameObject> targets)
        {
            Transform enemyTransform = stateMachine.transform;

            float closestSqrDist = Mathf.Infinity;
            GameObject closest = null;

            for (int i = 0; i < targets.Count; i++)
            {
                var go = targets[i];
                if (!go) continue;
                if (!go.CompareTag("Player")) continue;

                Vector3 diff = go.transform.position - enemyTransform.position;
                float sqrDist = diff.sqrMagnitude;

                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    closest = go;
                }
            }

            return closest;
        }


        private GameObject FindClosestTarget(int detectedCount, Collider[] colType)
        {
            Transform enemyTransform = stateMachine.transform;
            float closestDistance = Mathf.Infinity;
            GameObject closestTarget = null;

            for (int i = 0; i < detectedCount; i++)
            {
                Collider hit = colType[i];
                if (hit == null) continue;
                if (!hit.CompareTag("Player")) continue;

                Vector3 difference = hit.transform.position - enemyTransform.position;

                float sqrDist = difference.sqrMagnitude;

                if (sqrDist < closestDistance)
                {
                    closestDistance = sqrDist;
                    closestTarget = hit.gameObject;
                }
                // TODO add FOV /LOS check
            }

            stateMachine.Player = closestTarget;
            return closestTarget;
        }

        protected void Move(Vector3 movement, float deltaTime)
        {
            var motion = stateMachine.ForceReceiver.Movement;
            stateMachine.Controller.Move((motion + movement) * deltaTime);
        }

        protected void Move(float deltaTime)
        {
            Move(Vector3.zero, deltaTime);
        }

        protected void RotateToPlayer(float deltaTime)
        {
            if (stateMachine.Player == null) return;


            Vector3 directionToPlayer = stateMachine.Player.transform.position - stateMachine.transform.position;
            directionToPlayer.y = 0f;
            directionToPlayer.Normalize();

            var targetRot = Quaternion.LookRotation(directionToPlayer);
            Transform t = stateMachine.transform;
            t.rotation = Quaternion.Slerp(t.rotation, targetRot,
                stateMachine.EnemyConfigSo.MovementData.RotationDampTime * deltaTime
            );
        }
    }
}