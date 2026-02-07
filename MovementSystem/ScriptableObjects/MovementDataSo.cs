using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/Movement/Movement Data")]
    public class MovementDataSo : ScriptableObject
    {
        [Header("Animation Damp Times")]
        [Tooltip("The duration of the crossfade between the two blend trees")]
        [SerializeField]
        private float crossFadeDurationBetweenBlendTrees = 0.1f;

        public float CrossFadeDurationBetweenBlendTrees => crossFadeDurationBetweenBlendTrees;

        [Tooltip(" The damp time of the animator parameters")] [SerializeField]
        private float locomotionAnimatorDampTime = 0.1f;

        public float LocomotionAnimatorDampTime => locomotionAnimatorDampTime;

        [Tooltip(" The damp time of the animator parameters")] [SerializeField]
        private float targetingAnimatorDampTime = 0.2f;

        public float TargetingAnimatorDampTime => targetingAnimatorDampTime;

        [Header("Movement Settings")]
        [Tooltip(" The time it takes to rotate towards movement direction")]
        [SerializeField]
        private float rotationDampTime = 0.1f;

        public float RotationDampTime => rotationDampTime;

        [Tooltip(" The speed at which the player moves when in free look mode")] [SerializeField]
        private float freeMovementSpeed;

        public float FreeMovementSpeed => freeMovementSpeed;

        [Tooltip(" The speed at which the player moves when in Sprinting")] [SerializeField]
        private float freeSprintSpeed;

        public float FreeSprintSpeed => freeSprintSpeed;

        [Header("Targeting Movement Settings")]
        [Tooltip(" The speed at which the player moves when in targeting mode")]
        [SerializeField]
        private float targetingMovementSpeed;

        public float TargetingMovementSpeed => targetingMovementSpeed;

        [Space(5)] [Header("Anim Names")] [Tooltip("Targeting blend tree Name")] [SerializeField]
        private string targetingBlendTreeName;

        [Tooltip("Targeting Forward Speed Param name")] [SerializeField]
        private string targetingForwardSpeedParamName;

        [Tooltip("Targeting Right Speed")] [SerializeField]
        private string targetingRightSpeedParamName;


        [Header("Movement Anims")] [Tooltip("Anim Blend Tree name or Anim Name")] [SerializeField]
        private string blendTreeName;

        [Tooltip(" Free Look Speed Param Name for Movement Animation Speed")] [SerializeField]
        private string freelookSpeedParamName;

        [Header("Anim Hashes")] [SerializeField, HideInInspector]
        private int freeLookSpeedParamHash;

        [SerializeField, HideInInspector] private int locomotionBlendTreeHash;
        [SerializeField, HideInInspector] private int targetingBlendTreeHash;
        [SerializeField, HideInInspector] private int targetingForwardSpeedHash;
        [SerializeField, HideInInspector] private int targetingRightSpeedHash;

        public int LocomotionBlendTreeHash => locomotionBlendTreeHash;
        public int FreeLookSpeedParamHash => freeLookSpeedParamHash;
        public int TargetingBlendTreeHash => targetingBlendTreeHash;
        public int TargetingForwardSpeedHash => targetingForwardSpeedHash;
        public int TargetingRightSpeedHash => targetingRightSpeedHash;

        private void RebuildAnimHash()
        {
            locomotionBlendTreeHash = string.IsNullOrWhiteSpace(blendTreeName)
                ? 0
                : Animator.StringToHash(blendTreeName);
            freeLookSpeedParamHash = string.IsNullOrWhiteSpace(freelookSpeedParamName)
                ? 0
                : Animator.StringToHash(freelookSpeedParamName);
            targetingBlendTreeHash = string.IsNullOrWhiteSpace(targetingBlendTreeName)
                ? 0
                : Animator.StringToHash(targetingBlendTreeName);
            targetingForwardSpeedHash = string.IsNullOrWhiteSpace(targetingForwardSpeedParamName)
                ? 0
                : Animator.StringToHash(targetingForwardSpeedParamName);
            targetingRightSpeedHash = string.IsNullOrWhiteSpace(targetingRightSpeedParamName)
                ? 0
                : Animator.StringToHash(targetingRightSpeedParamName);
        }

        private void OnEnable() => RebuildAnimHash();
#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();
#endif
    }
}