using UnityEngine;

namespace _Project.Systems.MovementSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "FallingLandingDataSo", menuName = "Scriptable Objects/Fall-Land System/Falling Landing Data")]
    public class FallLandDataSo:ScriptableObject
    {
        [Header("Falling - Landing Settings")] [Tooltip("Falling State Start Velocity")] [SerializeField]
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
    }
}