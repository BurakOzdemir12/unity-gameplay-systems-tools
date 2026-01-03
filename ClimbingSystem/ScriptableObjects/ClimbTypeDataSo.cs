using _Project.Systems.ClimbingSystem.Enums;
using _Project.Systems.ClimbingSystem.LedgeClimbing;
using _Project.Systems.ClimbingSystem.Structs;
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

        public AvatarTarget MatchedBodyPart
        {
            get => matchedBodyPart;
            protected set => matchedBodyPart = value;
        }

        [SerializeField] private float matchStartTime;
        public float MatchStartTime => matchStartTime;

        [SerializeField] private float matchTargetTime;
        public float MatchTargetTime => matchTargetTime;

        [SerializeField] private Vector3 matchPosWeight;
        public Vector3 MatchPosWeight => matchPosWeight;

        public bool MirrorAnim { get; protected set; } = false;

        public bool MatchesActionType(ParkourActionType type)
        {
            return this.actionType == type;
        }

        public virtual ParkourDecision Evaluate(float height, in LedgeHitData hit, Vector3 playerPos,
            Vector3 playerRight)
        {
            bool valid = hit.IsValidLedge && height >= minObstacleHeight && height <= maxObstacleHeight;
            return valid ? new ParkourDecision(true, false, matchedBodyPart) : ParkourDecision.Invalid;
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