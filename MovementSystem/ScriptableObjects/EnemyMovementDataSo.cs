using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyMovementData", menuName = "Scriptable Objects/Movement/Enemy Movement Data")]
    public class EnemyMovementDataSo : MovementDataSo
    {
        [Space(5)]
        [Header("Enemy Specific Movement Settings")]
        [Header("Chase")]
        [Tooltip("Chase Range")]
        [SerializeField]
        private float chaseDetectionRange;

        public float ChaseDetectionRange => chaseDetectionRange;

        [Tooltip("Chase Detection Layers")]
        [field: SerializeField]
        public LayerMask ChaseDetectionLayers { get; private set; }

        [Header("Animation")] [Tooltip("The duration time of the locomotion blend tree ")] [SerializeField]
        private float locomotionBlendTreeDuration = 0.1f;

        public float LocomotionBlendTreeDuration => locomotionBlendTreeDuration;

        [Tooltip("The duration time for the Combat blend tree ")] [SerializeField]
        private float crossFadeDuration = 0.1f;

        public float CrossFadeDuration => crossFadeDuration;
    }
}