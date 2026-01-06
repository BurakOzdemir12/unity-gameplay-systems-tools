using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat.Dodge_Roll
{
    [CreateAssetMenu(fileName = "RollData", menuName = "Scriptable Objects/Combat/Roll Data")]
    public class RollDataSo:ScriptableObject
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
    }
}