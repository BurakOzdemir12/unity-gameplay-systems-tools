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
    }
}