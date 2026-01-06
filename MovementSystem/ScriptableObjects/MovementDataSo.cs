using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName ="MovementData" ,menuName = "Scriptable Objects/Movement/Movement Data")]
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

        [Header("Targeting Settings")]
        [Tooltip(" The speed at which the player moves when in targeting mode")]
        [SerializeField]
        private float targetingMovementSpeed;

        public float TargetingMovementSpeed => targetingMovementSpeed;
        
        
    }
}