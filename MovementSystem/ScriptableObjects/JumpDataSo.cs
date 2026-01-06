using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "JumpData", menuName = "Scriptable Objects/Jumping/Jump Data")]
    public class JumpDataSo : ScriptableObject
    {
        [Header("Jump Settings")] [Tooltip("Jump Force")] [SerializeField]
        private float jumpForce = 10f;

        public float JumpForce => jumpForce;

        [Tooltip("Jump Cooldown")] [SerializeField]
        private float jumpCooldownTime = 2f;

        public float JumpCooldownTime => jumpCooldownTime;
    }
}