using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat
{
    [CreateAssetMenu(fileName = "CombatData", menuName = "Scriptable Objects/Combat/Combat Data")]
    public class CombatDataSo : ScriptableObject
    {
        [Header("Attack settings")] [Tooltip(" Attack damage")] [SerializeField]
        private float attackDamage = 20f;

        public float AttackDamage => attackDamage;

        [Tooltip(" The time it takes to rotate towards movement direction")] [SerializeField]
        private float rotationDampTimeWhileAttack = 0.1f;

        public float RotationDampTimeWhileAttack => rotationDampTimeWhileAttack;

        [Header("Impact Settings")] [Tooltip(" Impact duration")] [SerializeField]
        private float impactDuration = 0.1f;

        public float ImpactDuration => impactDuration;

        [Header("Blocking Settings")] [Tooltip("The Blocking layer Change speed ")] [SerializeField]
        private float blockingLayerChangeSpeed = 0.1f;

        public float BlockingLayerChangeSpeed => blockingLayerChangeSpeed;


        [Tooltip(" The time it takes to rotate towards movement direction when Block")] [SerializeField]
        private float rotationDampTimeWhileBlock = 2f;

        public float RotationDampTimeWhileBlock => rotationDampTimeWhileBlock;

        [Header("Anim Names")] [Tooltip("Draw Sword Anim name")] [SerializeField]
        private string drawAnimName;

        public string DrawAnimName => drawAnimName;

        [Tooltip("Sheat Sword Anim name")] [SerializeField]
        private string sheatAnimName;

        public string SheatAnimName => sheatAnimName;

        [Tooltip("Armed Locomotion blendTree")] [SerializeField]
        private string combatIdleAnimName;

        public string CombatIdleAnimName => combatIdleAnimName;

        [Tooltip("Blocking Animation Name")] [SerializeField]
        private string blockAnimName;

        public string BlockAnimName => blockAnimName;

        [Tooltip("isBlocking boolean Parameter")] [SerializeField]
        private string isBlockingParamName;

        public string IsBlockingParamName => isBlockingParamName;

        // ArmedLocomBlendTree
        [Header("Anim Hashes")] [SerializeField, HideInInspector]
        private int sheatAnimHash;

        [SerializeField, HideInInspector] private int drawAnimHash;
        [SerializeField, HideInInspector] private int combatIdleAnimHash;
        [SerializeField, HideInInspector] private int blockAnimHash;
        [SerializeField, HideInInspector] private int isBlockingParamHash;
        public int SheatAnimHash => sheatAnimHash;
        public int DrawAnimHash => drawAnimHash;
        public int CombatIdleAnimHash => combatIdleAnimHash;
        public int BlockAnimHash => blockAnimHash;
        public int IsBlockingParamHash => isBlockingParamHash;

        private void RebuildAnimHash()
        {
            drawAnimHash = string.IsNullOrWhiteSpace(drawAnimName)
                ? 0
                : Animator.StringToHash(drawAnimName);

            sheatAnimHash = string.IsNullOrWhiteSpace(sheatAnimName)
                ? 0
                : Animator.StringToHash(sheatAnimName);

            combatIdleAnimHash = string.IsNullOrWhiteSpace(combatIdleAnimName)
                ? 0
                : Animator.StringToHash(combatIdleAnimName);

            blockAnimHash = string.IsNullOrWhiteSpace(blockAnimName)
                ? 0
                : Animator.StringToHash(blockAnimName);

            isBlockingParamHash = string.IsNullOrWhiteSpace(isBlockingParamName)
                ? 0
                : Animator.StringToHash(isBlockingParamName);
        }

        private void OnEnable() => RebuildAnimHash();

#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();

#endif
    }
}