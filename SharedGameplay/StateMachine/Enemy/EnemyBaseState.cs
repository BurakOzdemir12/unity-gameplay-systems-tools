using System.Collections.Generic;
using System.Linq;
using _Project.Systems._Core.StateMachine;
using _Project.Systems.CombatSystem.Enemy.States;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.StateMachine.Enemy
{
    public abstract class EnemyBaseState : State
    {
        protected readonly EnemyStateMachine stateMachine;
        private readonly Collider[] attackBuffer = new Collider[10];

        protected EnemyBaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        protected bool IsInAttackRange()
        {
            Vector3 attackPos =
                stateMachine.transform.TransformPoint(stateMachine.EnemyConfigSo.CombatData.AttackPositionOffset);
            int detected = Physics.OverlapSphereNonAlloc(
                attackPos,
                stateMachine.EnemyConfigSo.CombatData.AttackRange,
                attackBuffer,
                stateMachine.EnemyConfigSo.CombatData.AttackDetectionLayers,
                QueryTriggerInteraction.Ignore);
            
            stateMachine.BuffersForAttack.Clear();
            if (detected == 0)
            {
                stateMachine.BuffersForAttack.Clear();
#if UNITY_EDITOR
                stateMachine.debugBuffersForAttack.Clear();
#endif
                return false;
            }

            for (int i = 0; i < detected; i++)
            {
                if (attackBuffer[i] != null)
                {
                    stateMachine.BuffersForAttack.Add(attackBuffer[i]);
                }
            }
#if UNITY_EDITOR
            stateMachine.debugBuffersForAttack = stateMachine.BuffersForAttack.ToList();

#endif
            GameObject closestTarget = FindClosestTarget(stateMachine.BuffersForAttack);

            return closestTarget != null;
        }

        protected bool IsInChaseRange()
        {
            stateMachine.BuffersForChase.Clear();
            // Locked On Target Check -> if get hit enemy chase that target
            if (stateMachine.BuffersLockTarget.Count > 0)
            {
                // İf Target is dead remove it from lock target list
                stateMachine.BuffersLockTarget.RemoveWhere(col => col == null || !col.enabled);
#if UNITY_EDITOR
                stateMachine.debugBuffersForLockTarget.Clear();
                stateMachine.debugBuffersForLockTarget = stateMachine.BuffersLockTarget.ToList();
#endif
                // İf Target is alive then add it to chase Buffer Hashset
                if (stateMachine.BuffersLockTarget.Count > 0)
                {
                    stateMachine.BuffersForChase.UnionWith(stateMachine.BuffersLockTarget);

                    GameObject closestLockedTarget = FindClosestTarget(stateMachine.BuffersLockTarget);
                    stateMachine.Player = closestLockedTarget;
                    if (stateMachine.Player != null) return true;
                }
            }

            //Field Of View Check (FOV) -< If no locked target, checks FOV
            var targets = stateMachine.FieldOfView.Targets;
            if (targets == null || targets.Count == 0)
            {
                stateMachine.Player = null;
                stateMachine.BuffersForChase.Clear();
#if UNITY_EDITOR
                stateMachine.debugBuffersForChase.Clear();
#endif
                return false;
            }

            foreach (var target in targets)
            {
                if (target.TryGetComponent<Collider>(out var coll))
                {
                    stateMachine.BuffersForChase.Add(coll);
                }
            }
#if UNITY_EDITOR
            stateMachine.debugBuffersForChase = stateMachine.BuffersForChase.ToList();
#endif

            GameObject closestTarget = FindClosestTarget(stateMachine.BuffersForChase);
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


        private GameObject FindClosestTarget(HashSet<Collider> targets)
        {
            if (targets.Count == 0) return null;

            Transform enemyTransform = stateMachine.transform;
            float closestDistance = Mathf.Infinity;
            GameObject closestTarget = null;

            foreach (var hit in targets)
            {
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

        protected void HandleBlocking(float deltaTime, bool allowBlocking = true)
        {
            if (stateMachine.CurrentState is EnemyDeadState or EnemyAttackingState or EnemyParryState)
            {
                return;
            }


            if (!stateMachine.ShieldHandler.CurrentShieldHitbox)
            {
                stateMachine.Animator.SetBool(stateMachine.EnemyConfigSo.CombatData.IsBlockingParamHash, false);
                return;
            }

            if (!stateMachine.ShieldHandler.CurrentShieldLogic)
            {
                stateMachine.Animator.SetBool(stateMachine.EnemyConfigSo.CombatData.IsBlockingParamHash, false);
                stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex,
                    0f
                );
                return;
            }


            bool wantsBlock = allowBlocking && stateMachine.EnemyDefenceBrain.canBlockAttack;

            if (wantsBlock)
            {
                stateMachine.ShieldHandler.EnableShield();
            }
            else
            {
                stateMachine.ShieldHandler.DisableShield();
            }

            stateMachine.Animator.SetBool(stateMachine.EnemyConfigSo.CombatData.IsBlockingParamHash, wantsBlock);

            float targetWeight = wantsBlock ? 1f : 0f;

            stateMachine.blockLayerWeight = Mathf.MoveTowards(
                stateMachine.blockLayerWeight,
                targetWeight,
                deltaTime * stateMachine.EnemyConfigSo.CombatData.BlockingLayerChangeSpeed
            );

            stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex,
                stateMachine.blockLayerWeight
            );
        }
    }
}