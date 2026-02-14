using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyMovementData", menuName = "Scriptable Objects/Movement/Enemy Movement Data")]
    public class EnemyMovementDataSo : MovementDataSo
    {
        [field: Space(5)]
        [field: Header("Enemy Specific Movement Settings")]

        #region Suspicious Settings

        [field: Header("Suspicious Settings")]
        [field: Tooltip("Suspicious Walk Speed")]
        [SerializeField]
        private float suspiciousWalkSpeed = 3f;

        public float SuspiciousWalkSpeed => suspiciousWalkSpeed;

        [Tooltip("Enemy recognition to Target Time ")] [SerializeField]
        private float recognitionTime = 2f;

        public float RecognitionTime => recognitionTime;

        #endregion

        [field: Header("Chase Settings")]
        [Tooltip("Chase Detection Layers")]
        [field: SerializeField]
        public LayerMask ChaseDetectionLayers { get; private set; }

        [Tooltip("Enemy chase the target if is distance is less than this value")] [field: SerializeField]
        private float instantChaseDistance = 2f;

        public float InstantChaseDistance => instantChaseDistance;

        [Header("Animation")] [Tooltip("The duration time of the locomotion blend tree ")] [SerializeField]
        private float locomotionBlendTreeDuration = 0.1f;

        public float LocomotionBlendTreeDuration => locomotionBlendTreeDuration;

        [Tooltip("The duration time for the Combat blend tree ")] [SerializeField]
        private float crossFadeDuration = 0.1f;

        public float CrossFadeDuration => crossFadeDuration;

        #region Patrol Settings

        [Header("Patrol Settings")] [Tooltip("Patrol Range")] [SerializeField]
        private float patrolRange = 41.2f;

        public float PatrolRange => patrolRange;

        [Tooltip("Patrol Duration")] [SerializeField]
        private float patrolDuration = 10f;

        public float PatrolDuration => patrolDuration;

        [Tooltip("Patrol Cooldown")] [SerializeField]
        private float patrolCooldown = 10f;

        public float PatrolCooldown => patrolCooldown;

        #endregion

        #region Anim & Param Names

        [Header("Movement Names")] [Tooltip("Suspicious Anim Name")] [SerializeField]
        private string suspiciousBlendTreeName;

        #endregion

        #region Hash Convertion

        public int SuspiciousBlendTreeHash { get; private set; }

        protected override void RebuildAnimHash()
        {
            base.RebuildAnimHash();

            SuspiciousBlendTreeHash = string.IsNullOrWhiteSpace(suspiciousBlendTreeName)
                ? 0
                : Animator.StringToHash(suspiciousBlendTreeName);
        }

        #endregion
    }
}