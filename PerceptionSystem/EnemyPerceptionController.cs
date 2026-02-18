using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.PerceptionSystem.Enums;
using _Project.Systems.PerceptionSystem.Field_of_View;
using _Project.Systems.PerceptionSystem.Noise_Sensor;
using _Project.Systems.PerceptionSystem.Structs;
using _Project.Systems.SharedGameplay.BaseScriptableObjects.Characters;
using UnityEngine;

namespace _Project.Systems.PerceptionSystem
{
    public class EnemyPerceptionController : MonoBehaviour
    {
        [SerializeField] private EnemyConfigSo enemyConfig;
        [SerializeField] private FieldOfView fieldOfView;
        [SerializeField] private NoiseSensor noiseSensor;
        [SerializeField] private GameObject ownerGameObject;

        [Tooltip("Chase and Attack detect buffer length")] 
        [SerializeField] private int bufferMax = 4;

        [Header("Enemy Properties")] 
        [Tooltip("Is enemy deaf")] 
        [SerializeField] private bool isDeaf = false;

        [Tooltip("Is enemy blind")] 
        [SerializeField] private bool isBlind = false;

        [Header("Collider Buffers For detection")]
#if UNITY_EDITOR
        public List<Collider> debugBuffersForChase;
        public List<Collider> debugBuffersForAttack;
        public List<Collider> debugBuffersForLockTarget;
#endif
        [field: SerializeField] public GameObject CurrentTarget { get; private set; }
        public Vector3 LastKnownTargetPos { get; private set; }
        public bool IsTargetInAttackRange { get; private set; }
        public bool IsTargetInChaseRange { get; private set; }
        public bool HasSuspiciousTarget => bufferSetForLockTarget.Count > 0;
        [field: SerializeField] public bool IsAggressive { get; set; }

        //Collider Buffers
        private HashSet<Collider> bufferSetForChase;
        private HashSet<Collider> bufferSetForAttack;
        private HashSet<Collider> bufferSetForLockTarget;

        //Dynamic collider array
        private readonly Collider[] attackBuffer = new Collider[10];

        [Tooltip("Noise will not listen to target if heard within this time")] 
        [SerializeField] private float noiseHearingDelay = 2f;

        private float targetPersistenceTimer;
        private float lastNoiseHeardTime;

#if UNITY_EDITOR
        [SerializeField] private float debugTimer;
#endif

        //Perception Settings
        public event Action<PerceptionState, float> OnPerceptionChanged;
        private PerceptionState currentPerceptionState;
        private bool hasNewStimulus;

        private void OnEnable()
        {
            noiseSensor.OnNoiseHeard += HandleNoiseHeard;
        }

        private void Start()
        {
            bufferSetForChase = new HashSet<Collider>(bufferMax);
            bufferSetForAttack = new HashSet<Collider>(bufferMax);
            bufferSetForLockTarget = new HashSet<Collider>(bufferMax);
            currentPerceptionState = PerceptionState.Calm;
        }

        private void Update()
        {
            if (!enemyConfig) return;
            if (!isBlind)
            {
                CheckVisuals();
            }

            CheckAttackRange();
            ManageTargetVisibility();
            CheckAndBroadcastState();
        }

        public void Initialize(GameObject owner, EnemyConfigSo configData, FieldOfView fov, NoiseSensor noiseSensor)
        {
            this.ownerGameObject = owner;
            this.enemyConfig = configData;
            this.fieldOfView = fov;
            this.noiseSensor = noiseSensor;
        }

        //? Attack Range Check
        private void CheckAttackRange()
        {
            Vector3 attackPos = ownerGameObject.transform.TransformPoint(enemyConfig.CombatData.AttackPositionOffset);

            int detected = Physics.OverlapSphereNonAlloc(
                attackPos,
                enemyConfig.CombatData.AttackRange,
                attackBuffer,
                enemyConfig.CombatData.AttackDetectionLayers,
                QueryTriggerInteraction.Ignore);

            bufferSetForAttack.Clear();

            if (detected == 0)
            {
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

        private void CheckVisuals()
        {
            bufferSetForChase.Clear();
            
            // ! If enemy gets triggered by player then chase that player regardless of Range
            if (HandleLockedTargetChase()) return;

            //Field Of View Check (FOV)
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

            foreach (var target in targets)
            {
                if (target.TryGetComponent<Collider>(out var coll))
                {
                    bufferSetForChase.Add(coll);
                }
            }

            GameObject closestTarget = FindClosestTarget(bufferSetForChase);
            CurrentTarget = closestTarget;

            if (CurrentTarget != null)
            {
                LastKnownTargetPos = CurrentTarget.transform.position;
                
                // --- LOCK TARGET UPDATE ---
                if (CurrentTarget.TryGetComponent<Collider>(out var col))
                {
                    bufferSetForLockTarget.Add(col);
#if UNITY_EDITOR
                    if (!debugBuffersForLockTarget.Contains(col))
                        debugBuffersForLockTarget.Add(col);
#endif
                }
            }

            IsTargetInChaseRange = CurrentTarget != null;

#if UNITY_EDITOR
            debugBuffersForChase = bufferSetForChase.ToList();
#endif
        }

        //? If enemy gets hit by player, it will chase that target regardless of FOV and Range
        private bool HandleLockedTargetChase()
        {
            if (bufferSetForLockTarget.Count > 0)
            {
                bufferSetForLockTarget.RemoveWhere(col => col == null || !col.enabled);

#if UNITY_EDITOR
                debugBuffersForLockTarget.Clear();
                debugBuffersForLockTarget = bufferSetForLockTarget.ToList();
#endif

                if (bufferSetForLockTarget.Count > 0)
                {
                    bufferSetForChase.UnionWith(bufferSetForLockTarget);
                    GameObject closestLockedTarget = FindClosestTarget(bufferSetForLockTarget);
                    
                    CurrentTarget = closestLockedTarget;
                    if (CurrentTarget != null)
                    {
                        LastKnownTargetPos = CurrentTarget.transform.position;
                    }

                    IsTargetInChaseRange = closestLockedTarget != null;
                    return true;
                }
            }
            return false;
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
            if (damageInfoSourceObject != null && damageInfoSourceObject.TryGetComponent<Collider>(out var attackerCollider))
            {
                bufferSetForLockTarget.Add(attackerCollider);
#if UNITY_EDITOR
                debugBuffersForLockTarget = bufferSetForLockTarget.ToList();
#endif
            }
        }

        public bool IsTargetVisible(GameObject target)
        {
            if (target == null) return false;
            return fieldOfView.Targets.Contains(target);
        }

        public void OnDeath()
        {
            ResetPerception();
        }

        private void HandleNoiseHeard(NoiseData noiseData)
        {
            if (isDeaf) return;
            if (noiseData.Source == null) return;

            lastNoiseHeardTime = Time.time;
            LastKnownTargetPos = noiseData.Position;

            noiseData.Source.TryGetComponent<Collider>(out var col);
            bufferSetForLockTarget.Add(col);
            hasNewStimulus = true;

#if UNITY_EDITOR
            if (!debugBuffersForLockTarget.Contains(col))
            {
                debugBuffersForLockTarget.Add(col);
            }
#endif
        }

        private void ManageTargetVisibility()
        {
            if (bufferSetForLockTarget.Count == 0)
            {
                targetPersistenceTimer = 0f;
#if UNITY_EDITOR
                debugTimer = 0f;
#endif
                return;
            }

            GameObject lockedTarget = null;
            foreach (var col in bufferSetForLockTarget)
            {
                if (col != null) lockedTarget = col.gameObject;
                break;
            }

            if (lockedTarget == null) return;

            // Fov check
            bool canSeeTarget = false;
            List<GameObject> targets = fieldOfView.Targets;
            foreach (var target in targets)
            {
                if (lockedTarget == target)
                {
                    canSeeTarget = true;
                    break;
                }
            }

            bool heardRecently = (Time.time - lastNoiseHeardTime) < noiseHearingDelay;

            //? If a target can be seen or heard recently, don't reset persistence
            if (canSeeTarget || heardRecently)
            {
                targetPersistenceTimer = 0f;
#if UNITY_EDITOR
                debugTimer = 0f;
#endif
            }
            else
            {
                targetPersistenceTimer += Time.deltaTime;
#if UNITY_EDITOR
                debugTimer = targetPersistenceTimer;
#endif
                if (targetPersistenceTimer >= enemyConfig.MovementData.TargetPersistenceMemory) ResetPerception();
            }
        }

        private void CheckAndBroadcastState()
        {
            PerceptionState newState = PerceptionState.Calm;

            if (CurrentTarget)
            {
                newState = PerceptionState.Alerted;
            }
            else if (HasSuspiciousTarget)
            {
                newState = PerceptionState.Suspicious;
            }
            else
            {
                newState = PerceptionState.Calm;
            }

            if (newState != currentPerceptionState || hasNewStimulus)
            {
                currentPerceptionState = newState;
                EnemyMovementDataSo movementData = enemyConfig.MovementData;
                OnPerceptionChanged?.Invoke(currentPerceptionState, movementData.RecognitionTime);
                hasNewStimulus = false;
            }
        }

        private void ResetPerception()
        {
            bufferSetForChase.Clear();
            bufferSetForAttack.Clear();
            bufferSetForLockTarget.Clear();
            CurrentTarget = null;
            IsTargetInChaseRange = false;
            IsAggressive = false;

            debugBuffersForAttack.Clear();
            debugBuffersForChase.Clear();
            debugBuffersForLockTarget.Clear();
            currentPerceptionState = PerceptionState.Calm;
            
            OnPerceptionChanged?.Invoke(currentPerceptionState, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.darkRed;
            Gizmos.DrawWireSphere(ownerGameObject.transform.TransformPoint(enemyConfig.CombatData.AttackPositionOffset),
                enemyConfig.CombatData.AttackRange);
        }

        private void OnDisable()
        {
            noiseSensor.OnNoiseHeard -= HandleNoiseHeard;
        }
    }
}