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

        [Header("Selection")]
        [Tooltip("Higher priority wins when multiple SOs are valid (e.g., CenterVault > NormalVault).")]
        [SerializeField]
        private int priority = 0;

        public int Priority => priority;

        [Header("Animation Settings")] [SerializeField]
        private string animName;

        [SerializeField] private string animTag;
        public string AnimTag => animTag;

        [Space(5)] [Tooltip("Climbing Up Hanging to top of the object")] [SerializeField]
        private string climbUpAnimName;

        [Tooltip("Free Hang Climbing Animation Name")] [SerializeField]
        private string freeHandClimbAnimName;

        [Space(5)] [Header("Obstacle Rules")] [SerializeField]
        private float minObstacleHeight;

        [SerializeField] private float maxObstacleHeight;

        [Header("Rotation Settings")] [SerializeField]
        public bool rotateToObstacle;

        [SerializeField] public float rotationSpeed;

        [Header("Target Matching")] [SerializeField]
        public bool enableTargetMatching = true;

        [SerializeField] private AvatarTarget matchedBodyPart = AvatarTarget.LeftHand;
        public AvatarTarget MatchedBodyPart => matchedBodyPart;

        [SerializeField, Range(0f, 1f)] private float matchStartTime;
        public float MatchStartTime => matchStartTime;

        [SerializeField, Range(0f, 1f)] private float matchTargetTime;
        public float MatchTargetTime => matchTargetTime;

        [SerializeField] private Vector3 matchPosWeight;
        public Vector3 MatchPosWeight => matchPosWeight;

        public bool MatchesActionType(ParkourActionType type)
        {
            return this.actionType == type;
        }

        [Tooltip("Step up Climb animations crossfade duration")] [SerializeField]
        private float climbCrossFadeDuration = 0.1f;

        public float ClimbCrossFadeDuration => climbCrossFadeDuration;

        public virtual ParkourDecision Evaluate(float height, in LedgeHitData hit)
        {
            bool valid = hit.IsValidLedge && height >= minObstacleHeight && height <= maxObstacleHeight;
            return valid
                ? new ParkourDecision(true, isCenter: false, mirror: false, matchedBodyPart)
                : ParkourDecision.Invalid;
        }

        #region Hash Convertion

        public int AnimHash { get; private set; }
        public int ClimbUpAnimHash { get; private set; }
        public int FreeHangClimbHash { get; private set; }

        private void RebuildAnimHash()
        {
            AnimHash = string.IsNullOrWhiteSpace(animName)
                ? 0
                : Animator.StringToHash(animName);
            ClimbUpAnimHash = string.IsNullOrWhiteSpace(climbUpAnimName)
                ? 0
                : Animator.StringToHash(climbUpAnimName);
            FreeHangClimbHash = string.IsNullOrWhiteSpace(freeHandClimbAnimName)
                ? 0
                : Animator.StringToHash(freeHandClimbAnimName);
        }

        private void OnEnable() => RebuildAnimHash();
#if UNITY_EDITOR
        private void OnValidate() => RebuildAnimHash();
#endif

        #endregion
    }
}