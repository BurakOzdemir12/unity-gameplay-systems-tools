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

        public ParkourActionType ActionType => actionType;

        [Header("Animation Settings")] [SerializeField]
        private string animName;

        [SerializeField, HideInInspector] private int animHash;
        public int AnimHash => animHash;

        [SerializeField] private string animTag;
        public string AnimTag => animTag;


        [Header("Obstacle Rules")] [SerializeField]
        private float minObstacleHeight;

        [SerializeField] private float maxObstacleHeight;

        [Header("Rotation Settings")] [SerializeField]
        public bool rotateToObstacle;

        [SerializeField] public float rotationSpeed;

        [Header("Target Matching")] [SerializeField]
        public bool enableTargetMatching = true;

        [SerializeField] private AvatarTarget matchedBodyPart;
        public AvatarTarget MatchedBodyPart => matchedBodyPart;

        [SerializeField] private float matchStartTime;
        public float MatchStartTime => matchStartTime;

        [SerializeField] private float matchTargetTime;
        public float MatchTargetTime => matchTargetTime;
        
        [SerializeField] private Vector3 matchPosWeight;
        public Vector3 MatchPosWeight => matchPosWeight;

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