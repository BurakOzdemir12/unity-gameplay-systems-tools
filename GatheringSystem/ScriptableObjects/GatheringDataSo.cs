using System;
using _Project.Systems._Core.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems.GatheringSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GatheringDataSo", menuName = "Scriptable Objects/Gathering System/GatheringDataSo")]
    public class GatheringDataSo : ScriptableObject
    {
        [Header("Mining Settings")] [Tooltip("Gatering Local Speed")]
        public float gatheringSpeed;

        [Tooltip("Gather Power/Level/damage")] public float gatheringPower;
        [Tooltip("Gather Action Type")] public GatherActionType gatherActionType;
        [Tooltip("Anim Hash")] private int animHash;
        public int AnimHash => animHash;
        [Tooltip("Anim Tag")] [SerializeField] private string animTag;
        public string AnimTag => animTag;
        [Tooltip("Cooldown For Gater Enter/Exit")]
        public float cooldown;

        [Tooltip("Cancel Gather by Move threshHold ")] public float cancelMoveThreshold;
        [Tooltip("Cancel Gather move input hold time")] public float cancelMoveHoldTime;

        private void RebuildAnimHash()
        {
            var animName = Enum.GetName(typeof(GatherActionType), gatherActionType);

            animHash = string.IsNullOrWhiteSpace(animName)
                ? 0
                : Animator.StringToHash(animName);
        }

        private void OnEnable() => RebuildAnimHash();

#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();

#endif
    }
}