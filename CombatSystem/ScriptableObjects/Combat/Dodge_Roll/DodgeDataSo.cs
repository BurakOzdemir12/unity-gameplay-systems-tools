using System;
using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat.Dodge_Roll
{
    [CreateAssetMenu(fileName = "DodgeData", menuName = "Scriptable Objects/Combat/Dodge Data")]
    public class DodgeDataSo : ScriptableObject
    {
        [Header("Dodge Settings")] [Tooltip("Dodge speed")] [SerializeField]
        private float dodgeSpeed = 10f;

        public float DodgeSpeed => dodgeSpeed;

        [Tooltip(" The time it takes to rotate towards movement direction")] [SerializeField]
        private float rotationDampTimeWhileDodge = 2f;

        public float RotationDampTimeWhileDodge => rotationDampTimeWhileDodge;

        [Tooltip("Dodge Cooldown Time")] [SerializeField]
        private float dodgeCooldownTime = 2f;

        public float DodgeCooldownTime => dodgeCooldownTime;


        [Tooltip("Dodge Animation Start Time")] [Range(0f, 1f)] [SerializeField]
        private float dodgeAnimStartTime;

        public float DodgeAnimStartTime => dodgeAnimStartTime;

        [Tooltip("Dodge Animation End Time")] [Range(0f, 1f)] [SerializeField]
        private float dodgeAnimEndTime;

        public float DodgeAnimEndTime => dodgeAnimEndTime;

        [Header("Dodge Anim & Param names")] [Tooltip("Dodge Backward Animation Param Name")] [SerializeField]
        private string dodgeBackwardAnimName;

        [Tooltip("Dodge Forward Animation Param Name")] [SerializeField]
        private string dodgeForwardAnimName;

        [Tooltip("Dodge Forward Animation Param Name")] [SerializeField]
        private string dodgeRightAnimName;

        [Tooltip("Dodge Forward Animation Param Name")] [SerializeField]
        private string dodgeLeftAnimName;

        #region Hash Convertion

        [Header("Anim Hashes")] 
        public int DodgeBackwardHash { get; private set; }
        public int DodgeForwardHash { get; private set; }
        public int DodgeRightHash { get; private set; }
        public int DodgeLeftHash { get; private set; }
        
        private void RebuildAnimHash()
        {
            DodgeBackwardHash = string.IsNullOrWhiteSpace(dodgeBackwardAnimName)
                ? 0
                : Animator.StringToHash(dodgeBackwardAnimName);
            DodgeForwardHash = string.IsNullOrWhiteSpace(dodgeForwardAnimName)
                ? 0
                : Animator.StringToHash(dodgeForwardAnimName);
            DodgeRightHash = string.IsNullOrWhiteSpace(dodgeRightAnimName)
                ? 0
                : Animator.StringToHash(dodgeRightAnimName);
            DodgeLeftHash = string.IsNullOrWhiteSpace(dodgeLeftAnimName)
                ? 0
                : Animator.StringToHash(dodgeLeftAnimName);
        }

        private void OnEnable() => RebuildAnimHash();
#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();
#endif

        #endregion
    }
}