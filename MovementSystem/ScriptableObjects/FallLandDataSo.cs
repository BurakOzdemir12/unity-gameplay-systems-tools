using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "FallingLandingDataSo",
        menuName = "Scriptable Objects/Fall-Land System/Falling Landing Data")]
    public class FallLandDataSo : ScriptableObject
    {
        [Header("Falling - Landing Settings")] [Tooltip(" Falling Animation Name")] [SerializeField]
        private string fallingAnimName;

        [SerializeField] private string landingAnimName;
        [SerializeField] private string landingHeavyAnimName;


        [Tooltip("Falling State Start Velocity")] [SerializeField]
        private float fallingHeightThreshold;

        public float FallingHeightThreshold => fallingHeightThreshold;


        [Tooltip("Landing State Start Heightens")] [SerializeField]
        private float landingHeightThreshold;

        public float LandingHeightThreshold => landingHeightThreshold;

        [Tooltip("Hard Landing  Start Heightens")] [SerializeField]
        private float landingHardHeightThreshold;

        public float LandingHardHeightThreshold => landingHardHeightThreshold;

        [Tooltip("Landing Animation Start Time")] [Range(0f, 2f)] [SerializeField]
        private float landingStateExitTime;

        public float LandingStateExitTime => landingStateExitTime;

        #region Hash Convertion

        //Anim Hashes
        public int FallingLoopHash { get; private set; }
        public int LandingHash { get; private set; }
        public int LandingHeavyHash { get; private set; }

        private void RebuildAnimHash()
        {
            FallingLoopHash = string.IsNullOrWhiteSpace(fallingAnimName)
                ? 0
                : Animator.StringToHash(fallingAnimName);
            LandingHash = string.IsNullOrWhiteSpace(landingAnimName)
                ? 0
                : Animator.StringToHash(landingAnimName);
            LandingHeavyHash = string.IsNullOrWhiteSpace(landingHeavyAnimName)
                ? 0
                : Animator.StringToHash(landingHeavyAnimName);
        }

        private void OnEnable() => RebuildAnimHash();
#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();
#endif

        #endregion
    }
}