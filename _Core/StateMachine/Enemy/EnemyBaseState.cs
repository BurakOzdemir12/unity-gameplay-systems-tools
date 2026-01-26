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

        protected bool IsInAttackRange()
        {
            Collider[] debugBuffer = new Collider[10];
            int detected = Physics.OverlapSphereNonAlloc(
                stateMachine.transform.position + stateMachine.EnemyConfigSo.CombatData.AttackPositionOffset,
                stateMachine.EnemyConfigSo.CombatData.AttackRange,
                debugBuffer,
                stateMachine.EnemyConfigSo.CombatData.AttackDetectionLayers,
                QueryTriggerInteraction.Ignore);
            if (detected == 0)

            {
                stateMachine.BuffersForAttack.Clear();
                stateMachine.debugBuffersForAttack.Clear();

                return false;
            }

            for (int i = 0; i < detected; i++)
            {
                if (debugBuffer[i].TryGetComponent<Collider>(out var coll))
                {
                    stateMachine.BuffersForAttack.Add(coll);
                }
            }

            stateMachine.debugBuffersForAttack = stateMachine.BuffersForAttack.ToList();
            GameObject closestTarget = FindClosestTarget(detected, stateMachine.BuffersForAttack);

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

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].TryGetComponent<Collider>(out var coll))
                {
                    stateMachine.BuffersForChase.Add(coll);
                }
            }

            stateMachine.debugBuffersForChase = stateMachine.BuffersForChase.ToList();
            GameObject closestTarget = FindClosestTarget(targets.Count, stateMachine.BuffersForChase);
            stateMachine.Player = closestTarget;
            return closestTarget != null;
        }

        // private GameObject FindClosestTargetForChase(List<GameObject> targets)
        // {
        //     Transform enemyTransform = stateMachine.transform;
        //
        //     float closestSqrDist = Mathf.Infinity;
        //     GameObject closest = null;
        //
        //     for (int i = 0; i < targets.Count; i++)
        //     {
        //         var go = targets[i];
        //         if (!go) continue;
        //         if (!go.CompareTag("Player")) continue;
        //
        //         Vector3 diff = go.transform.position - enemyTransform.position;
        //         float sqrDist = diff.sqrMagnitude;
        //
        //         if (sqrDist < closestSqrDist)
        //         {
        //             closestSqrDist = sqrDist;
        //             closest = go;
        //         }
        //     }
        //
        //     return closest;
        // }


        private GameObject FindClosestTarget(int detectedCount, HashSet<Collider> colType)
        {
            Transform enemyTransform = stateMachine.transform;
            float closestDistance = Mathf.Infinity;
            GameObject closestTarget = null;

            for (int i = 0; i < detectedCount; i++)
            {
                Collider hit = colType.ElementAt(i); //[i];
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
            Transform t = stateMachine.transform;

            if (stateMachine.Player)
            {
                Vector3 directionToPlayer = stateMachine.Player.transform.position - stateMachine.transform.position;
                directionToPlayer.y = 0f;
                directionToPlayer.Normalize();

                var targetRot = Quaternion.LookRotation(directionToPlayer);
                t.rotation = Quaternion.Slerp(t.rotation, targetRot,
                    stateMachine.EnemyConfigSo.MovementData.RotationDampTime * deltaTime
                );
                return;
            }

            Vector3 directionToTarget = stateMachine.Agent.steeringTarget - stateMachine.transform.position;
            directionToTarget.y = 0f;
            directionToTarget.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            t.rotation = Quaternion.Slerp(t.rotation, targetRotation,
                stateMachine.EnemyConfigSo.MovementData.RotationDampTime * deltaTime
            );
        }
    }
}