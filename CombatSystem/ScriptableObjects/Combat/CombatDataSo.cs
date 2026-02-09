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

        [Header("Anim Names")] [Space(5)] [Tooltip("Draw Sword Anim name")] [SerializeField]
        private string drawAnimName;

        [Tooltip("Sheat Sword Anim name")] [SerializeField]
        private string sheatAnimName;

        [Header("Combat Anim Names")] [Tooltip("Combat idle Anim Name")] [SerializeField]
        private string combatIdleAnimName;

        [Tooltip("Stunned Animation Name")] [SerializeField]
        private string stunnedAnimName;

        [Tooltip("Impact Slight Animation Name")] [SerializeField]
        private string impactSlightAnimName;

        [Tooltip("is Armed Boolean Param Name")] [SerializeField]
        private string isArmedParamName;

        [Space(5)] [Header("Block Anim Names")] [Tooltip("Blocking Animation Name")] [SerializeField]
        private string blockAnimName;

        [Tooltip("Block Impact Animation Name")] [SerializeField]
        private string blockImpactAnimName;

        [Tooltip("Block Pair Animation Name")] [SerializeField]
        private string blockPairAnimName;

        [Tooltip("isBlocking boolean Parameter ")] [SerializeField]
        private string isBlockingParamName;


        public int SheatAnimHash { get; private set; }
        public int DrawAnimHash { get; private set; }
        public int CombatIdleAnimHash { get; private set; }
        public int BlockAnimHash { get; private set; }
        public int BlockImpactAnimHash { get; private set; }
        public int BlockParryAnimHash { get; private set; }
        public int IsBlockingParamHash { get; private set; }
        public int StunnedAnimParamHash { get; private set; }
        public int ImpactSlightAnimHash { get; private set; }
        public int IsArmedParamHash { get; private set; }

        private void RebuildAnimHash()
        {
            DrawAnimHash = string.IsNullOrWhiteSpace(drawAnimName)
                ? 0
                : Animator.StringToHash(drawAnimName);

            SheatAnimHash = string.IsNullOrWhiteSpace(sheatAnimName)
                ? 0
                : Animator.StringToHash(sheatAnimName);

            CombatIdleAnimHash = string.IsNullOrWhiteSpace(combatIdleAnimName)
                ? 0
                : Animator.StringToHash(combatIdleAnimName);

            BlockAnimHash = string.IsNullOrWhiteSpace(blockAnimName)
                ? 0
                : Animator.StringToHash(blockAnimName);
            BlockImpactAnimHash = string.IsNullOrWhiteSpace(blockImpactAnimName)
                ? 0
                : Animator.StringToHash(blockImpactAnimName);
            BlockParryAnimHash = string.IsNullOrWhiteSpace(blockPairAnimName)
                ? 0
                : Animator.StringToHash(blockPairAnimName);

            IsBlockingParamHash = string.IsNullOrWhiteSpace(isBlockingParamName)
                ? 0
                : Animator.StringToHash(isBlockingParamName);

            StunnedAnimParamHash = string.IsNullOrWhiteSpace(stunnedAnimName)
                ? 0
                : Animator.StringToHash(stunnedAnimName);

            ImpactSlightAnimHash = string.IsNullOrWhiteSpace(impactSlightAnimName)
                ? 0
                : Animator.StringToHash(impactSlightAnimName);
            IsArmedParamHash = string.IsNullOrWhiteSpace(isArmedParamName)
                ? 0
                : Animator.StringToHash(isArmedParamName);
        }

        private void OnEnable() => RebuildAnimHash();

#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();

#endif
    }
}