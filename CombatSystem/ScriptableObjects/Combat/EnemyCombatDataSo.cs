using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat
{
    [CreateAssetMenu(fileName = "EnemyCombatData", menuName = "Scriptable Objects/Combat/Enemy Combat Data")]
    public class EnemyCombatDataSo : CombatDataSo
    {
        [Header("Enemy Specific Attack Settings")] [Tooltip("Attack Position Offset")] [SerializeField]
        private Vector3 attackPositionOffset;

        public Vector3 AttackPositionOffset => attackPositionOffset;

        [Tooltip("Attack Range")] [SerializeField]
        private float attackRange;

        public float AttackRange => attackRange;

        [Tooltip("Attack Detection Layers")] [SerializeField]
        private LayerMask attackDetectionLayers;

        public LayerMask AttackDetectionLayers => attackDetectionLayers;

        [Tooltip("Attack Cooldown Time")] [SerializeField]
        private float attackCoolDown;

        public float AttackCoolDown => attackCoolDown;

        [Tooltip("Attack Force Value")] [SerializeField]
        private float attackKnockBackForce;

        public float AttackKnockBackForce => attackKnockBackForce;

        [Tooltip("Attack Force Time")] [SerializeField]
        private float forceTime;

        public float ForceTime => forceTime;

        [Tooltip("The duration time for the Combat blend tree ")] [SerializeField]
        private float crossFadeDurationCombat = 0.1f;

        public float CrossFadeDurationCombat => crossFadeDurationCombat;

        [Header("Anim Names just for Enemy")] [Tooltip("Enemy Attack One Hand => Right  ")] [SerializeField]
        private string enemyAttack1RAnimName;

        public int EnemyAttack1RHash { get; private set; }

        protected override void RebuildAnimHash()
        {
            base.RebuildAnimHash();

            EnemyAttack1RHash = string.IsNullOrWhiteSpace(enemyAttack1RAnimName)
                ? 0
                : Animator.StringToHash(enemyAttack1RAnimName);
        }
    }
}