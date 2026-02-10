using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Systems.EnemyPerceptionSystem.Field_of_View;
using _Project.Systems.SharedGameplay.BaseScriptableObjects.Characters;
using UnityEngine;

namespace _Project.Systems.EnemyPerceptionSystem
{
    public class EnemyPerceptionController : MonoBehaviour
    {
        [SerializeField] private EnemyConfigSo enemyConfig;
        [SerializeField] private FieldOfView fieldOfView;
        [SerializeField] private GameObject ownerGameObject;

        [Tooltip("Chase and Attack detect buffer length")] [SerializeField]
        private int bufferMax = 4;

        //Collider Buffers
        private HashSet<Collider> bufferSetForChase;
        private HashSet<Collider> bufferSetForAttack;
        private HashSet<Collider> bufferSetForLockTarget;

        //Dynamic collider array
        private readonly Collider[] attackBuffer = new Collider[10];

        [Header("Collider Buffers For detection")]
#if UNITY_EDITOR
        public List<Collider> debugBuffersForChase;

        public List<Collider> debugBuffersForAttack;
        public List<Collider> debugBuffersForLockTarget;
#endif
        public GameObject CurrentTarget { get; set; }
        public bool IsTargetInAttackRange { get; private set; }
        public bool IsTargetInChaseRange { get; private set; }


        private void Start()
        {
            bufferSetForChase = new HashSet<Collider>(bufferMax);
            bufferSetForAttack = new HashSet<Collider>(bufferMax);
            bufferSetForLockTarget = new HashSet<Collider>(bufferMax);
        }

        private void Update()
        {
            if (!enemyConfig) return;
            IsInChaseRange();
            IsInAttackRange();
        }

        public void Initialize(GameObject owner, EnemyConfigSo configData, FieldOfView fov)
        {
            this.ownerGameObject = owner;
            this.enemyConfig = configData;
            this.fieldOfView = fov;
        }

        private void IsInAttackRange()
        {
            Vector3 attackPos =
                ownerGameObject.transform.TransformPoint(enemyConfig.CombatData.AttackPositionOffset);
            int detected = Physics.OverlapSphereNonAlloc(
                attackPos,
                enemyConfig.CombatData.AttackRange,
                attackBuffer,
                enemyConfig.CombatData.AttackDetectionLayers,
                QueryTriggerInteraction.Ignore);

            bufferSetForAttack.Clear();
            if (detected == 0)
            {
                bufferSetForAttack.Clear();
                IsTargetInAttackRange = false;
#if UNITY_EDITOR
                debugBuffersForAttack.Clear();
#endif
                return;
            }

            for (int i = 0; i < detected; i++)
            {
                if (attackBuffer[i] != null)
                {
                    bufferSetForAttack.Add(attackBuffer[i]);
                }
            }
#if UNITY_EDITOR
            debugBuffersForAttack = bufferSetForAttack.ToList();

#endif
            GameObject closestTarget = FindClosestTarget(bufferSetForAttack);
            IsTargetInAttackRange = closestTarget != null && closestTarget == CurrentTarget;
        }

        private void IsInChaseRange()
        {
            bufferSetForChase.Clear();
            // Locked On Target Check -> if get hit enemy chase that target
            if (bufferSetForLockTarget.Count > 0)
            {
                // İf Target is dead remove it from lock target list
                bufferSetForLockTarget.RemoveWhere(col => col == null || !col.enabled);
#if UNITY_EDITOR
                debugBuffersForLockTarget.Clear();
                debugBuffersForLockTarget = bufferSetForLockTarget.ToList();
#endif
                // İf Target is alive then add it to chase Buffer Hashset
                if (bufferSetForLockTarget.Count > 0)
                {
                    bufferSetForChase.UnionWith(bufferSetForLockTarget);

                    GameObject closestLockedTarget = FindClosestTarget(bufferSetForLockTarget);
                    CurrentTarget = closestLockedTarget;
                    IsTargetInChaseRange = closestLockedTarget != null;
                    return;
                }
            }

            //Field Of View Check (FOV) -< If no locked target, checks FOV
            var targets = fieldOfView.Targets;
            if (targets == null || targets.Count == 0)
            {
                CurrentTarget = null;
                bufferSetForChase.Clear();
#if UNITY_EDITOR
                debugBuffersForChase.Clear();
#endif
                return;
            }

            CurrentTarget = null;
            IsTargetInChaseRange = false;
            IsTargetInChaseRange = false;
            foreach (var target in targets)
            {
                if (target.TryGetComponent<Collider>(out var coll))
                {
                    bufferSetForChase.Add(coll);
                }
            }

            GameObject closestTarget = FindClosestTarget(bufferSetForChase);
            CurrentTarget = closestTarget;
            IsTargetInChaseRange = CurrentTarget != null;

#if UNITY_EDITOR
            debugBuffersForChase = bufferSetForChase.ToList();
#endif
        }


        private GameObject FindClosestTarget(HashSet<Collider> targets)
        {
            if (targets.Count == 0) return null;

            Transform enemyTransform = transform;
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

            CurrentTarget = closestTarget;
            return closestTarget;
        }

        public bool IsPerceivingTarget(GameObject target)
        {
            if (target == null) return false;

            Transform targetRoot = target.transform.root;

            foreach (var col in bufferSetForAttack)
            {
                if (col != null && col.transform.root == targetRoot)
                {
                    return true;
                }
            }

            return false;
        }

        public void OnDamageTaken(GameObject damageInfoSourceObject)
        {
            if (damageInfoSourceObject != null &&
                damageInfoSourceObject.TryGetComponent<Collider>(out var attackerCollider))
            {
                bufferSetForLockTarget.Add(attackerCollider);
#if UNITY_EDITOR
                debugBuffersForLockTarget = bufferSetForLockTarget.ToList();
#endif
            }
        }

        public void OnDeath()
        {
            bufferSetForChase.Clear();
            bufferSetForAttack.Clear();
            bufferSetForLockTarget.Clear();
            CurrentTarget = null;
            IsTargetInChaseRange = false;
            IsTargetInChaseRange = false;
            debugBuffersForAttack.Clear();
            debugBuffersForChase.Clear();
            debugBuffersForLockTarget.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.darkRed;
            Gizmos.DrawWireSphere(ownerGameObject.transform.TransformPoint(enemyConfig.CombatData.AttackPositionOffset),
                enemyConfig.CombatData.AttackRange);
        }
    }
}