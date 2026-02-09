using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "JumpData", menuName = "Scriptable Objects/Jumping/Jump Data")]
    public class JumpDataSo : ScriptableObject
    {
        [Header("Jump Anims Params")] [Tooltip("Idle to Jump Anim name")] [SerializeField]
        private string jumpAnimName;

        [Header("Jump Settings")] [Tooltip("Jump Force")] [SerializeField]
        private float jumpForce = 10f;

        public float JumpForce => jumpForce;

        [Tooltip("Jump Cooldown")] [SerializeField]
        private float jumpCooldownTime = 2f;

        public float JumpCooldownTime => jumpCooldownTime;

        #region Hash Convertion

        [Header("Anim Hashes")] public int JumpAnimHash { get; private set; }

        private void RebuildAnimHash()
        {
            JumpAnimHash = string.IsNullOrWhiteSpace(jumpAnimName)
                ? 0
                : Animator.StringToHash(jumpAnimName);
        }

        private void OnEnable() => RebuildAnimHash();
#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();
#endif

        #endregion
    }
}