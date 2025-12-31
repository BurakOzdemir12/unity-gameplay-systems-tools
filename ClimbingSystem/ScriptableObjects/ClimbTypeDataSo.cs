using _Project.Systems.ClimbingSystem.Enums;
using _Project.Systems.ClimbingSystem.LedgeClimbing;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ParkourControllerSo",
        menuName = "Scriptable Objects/Climb System/ParkourControllerSO")]
    public class ClimbTypeDataSo : ScriptableObject
    {
        [Header("Action")] [SerializeField] private ParkourActionType actionType;

        [Header("Animation Settings")] [SerializeField]
        private string animName;

        [SerializeField, HideInInspector] private int animHash;
        [SerializeField] private string animTag;

        [Header("Obstacle Rules")] [SerializeField]
        private float minObstacleHeight;

        [SerializeField] private float maxObstacleHeight;


        public ParkourActionType ActionType => actionType;

        public int AnimHash => animHash;
        public string AnimTag => animTag;

        public bool CheckLedgeHeight(float height)
        {
            return height >= minObstacleHeight && height <= maxObstacleHeight;
        }

        private void OnEnable()
        {
            // Build Protection
            if (animHash == 0 && !string.IsNullOrWhiteSpace(animName))
                animHash = Animator.StringToHash(animName);
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            animHash = Animator.StringToHash(animName);
        }
#endif
    }
}