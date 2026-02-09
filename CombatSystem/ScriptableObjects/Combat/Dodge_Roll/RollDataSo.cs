using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat.Dodge_Roll
{
    [CreateAssetMenu(fileName = "RollData", menuName = "Scriptable Objects/Combat/Roll Data")]
    public class RollDataSo : ScriptableObject
    {
        [Header("Roll Settings")] [Tooltip("Roll Speed")] [SerializeField]
        private float rollSpeed = 2f;

        public float RollSpeed => rollSpeed;

        [Tooltip(" The time it takes to rotate towards movement direction while Roll")] [SerializeField]
        private float rotationDampTimeWhileRoll = 2f;

        public float RotationDampTimeWhileRoll => rotationDampTimeWhileRoll;

        [Tooltip("Roll Cooldown")] [SerializeField]
        private float rollCooldownTime = 2f;

        public float RollCooldownTime => rollCooldownTime;


        [Tooltip("Roll Animation Start Time")] [Range(0f, 1f)] [SerializeField]
        private float rollAnimStartTime;

        public float RollAnimStartTime => rollAnimStartTime;

        [Tooltip("Roll Animation End Time")] [Range(0f, 1f)] [SerializeField]
        private float rollAnimEndTime;

        public float RollAnimEndTime => rollAnimEndTime;

        [Header("Roll Anim & Param names")] [Tooltip("Roll Backward Animation Param Name")] [SerializeField]
        private string rollBackwardAnimName;

        [Tooltip("Roll Forward Animation Param Name")] [SerializeField]
        private string rollForwardAnimName;

        [Tooltip("Roll Forward Animation Param Name")] [SerializeField]
        private string rollRightAnimName;

        [Tooltip("Roll Forward Animation Param Name")] [SerializeField]
        private string rollLeftAnimName;

        #region Hashe Convertion

        [Header("Anim Hashes")] public int RollBackwardHash { get; private set; }

        public int RollForwardHash { get; private set; }
        public int RollRightHash { get; private set; }
        public int RollLeftHash { get; private set; }


        private void RebuildAnimHash()
        {
            RollBackwardHash = string.IsNullOrWhiteSpace(rollBackwardAnimName)
                ? 0
                : Animator.StringToHash(rollBackwardAnimName);
            RollForwardHash = string.IsNullOrWhiteSpace(rollForwardAnimName)
                ? 0
                : Animator.StringToHash(rollForwardAnimName);
            RollRightHash = string.IsNullOrWhiteSpace(rollRightAnimName)
                ? 0
                : Animator.StringToHash(rollRightAnimName);
            RollLeftHash = string.IsNullOrWhiteSpace(rollLeftAnimName)
                ? 0
                : Animator.StringToHash(rollLeftAnimName);
        }

        private void OnEnable() => RebuildAnimHash();
#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();
#endif

        #endregion
    }
}