using _Project.Core.Scripts;
using _Project.Systems._Core.GravityForce;
using _Project.Systems._Core.GroundCheck;
using _Project.Systems._Core.Health;
using _Project.Systems._Core.ScriptableObjects.Characters;
using _Project.Systems.ClimbingSystem.LedgeClimbing;
using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.CombatSystem.Player.States;
using _Project.Systems.CombatSystem.ScriptableObjects;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.CombatSystem.Targeting;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.MovementSystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.StateMachine.Player
{
    public class PlayerStateMachine : StateMachine
    {
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponLogic.WeaponLogic WeaponLogic { get; private set; }
        [field: SerializeField] public ToolLogic.ToolLogic ToolLogic { get; private set; }
        [field: SerializeField] public PlayerHealth Health { get; private set; }
        [field: SerializeField] public Ragdoll.Ragdoll Ragdoll { get; private set; }
        [field: SerializeField] public ClimbController ClimbController { get; private set; }
        [field: SerializeField] public GroundChecker GroundChecker { get; private set; }

        [field: SerializeField] public PlayerConfigSo PlayerConfigSo { get; private set; }

        // [Header("Weapon Transforms")] [Tooltip("Sword Holder Transform")] [field: SerializeField]
        // public Transform swordHolderR;

        [Header("Blocking Settings")] [field: SerializeField]
        public float blockLayerWeight = 1;

        [Header("Animation Settings")] [Tooltip("The duration time of the locomotion blend tree ")] [SerializeField]
        private float crossFadeDuration = 0.1f;

        public float CrossFadeDuration => crossFadeDuration;

        [Header("Dodge Settings")] [Tooltip("Dodge Press Previous Time ")]
        private float previousDodgeTime;

        public float PreviousDodgeTime
        {
            get => previousDodgeTime;
            set => previousDodgeTime = value;
        }

        [Tooltip("Previous time of roll animation")]
        private float previousRollTime;

        public float PreviousRollTime
        {
            get => previousRollTime;
            set => previousRollTime = value;
        }

        [Header("Jump Settings")]
        [field: Tooltip("Previous time of jump animation")]
        public bool PendingJump { get; private set; }

        private float previousJumpTime;

        public float PreviousJumpTime
        {
            get => previousJumpTime;
            set => previousJumpTime = value;
        }

        [Tooltip("is Climbing Free Flow")]
        [field: SerializeField]
        public bool IsFreeFlowClimb { get; private set; }


        [Tooltip("İf Your animations works with the root motion, set this to true")] [SerializeField]
        public bool workWithRootMotion = false;

        [Tooltip("In Combat or alert mode, its gonna roll but in normal mode jump will work ")] [SerializeField]
        private bool inAlertMode = false;

        public bool IsInAlertMode
        {
            get => inAlertMode;
            set => inAlertMode = value;
        }

        //TODO Use animation override controller or create AnimationProfileSo for Hashes 

        public readonly int FreeLookSpeedParamHash = Animator.StringToHash("FreeLookSpeed");
        public readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
        public readonly int TargetingBlendTreeHash = Animator.StringToHash("TargetingBlendTree");
        public readonly int TargetingForwardSpeedHash = Animator.StringToHash("TargetingForwardSpeed");
        public readonly int TargetingRightSpeedHash = Animator.StringToHash("TargetingRightSpeed");
        public readonly int LightImpactHash = Animator.StringToHash("ImpactSlight");
        public readonly int IsBlockingBoolHash = Animator.StringToHash("isBlocking");
        public readonly int DodgeBackwardHash = Animator.StringToHash("Dodge Backward");
        public readonly int DodgeForwardHash = Animator.StringToHash("Dodge Forward");
        public readonly int DodgeRightHash = Animator.StringToHash("Dodge Right");
        public readonly int DodgeLeftHash = Animator.StringToHash("Dodge Left");
        public readonly int RollForwardHash = Animator.StringToHash("Roll Forward");
        public readonly int RollBackwardHash = Animator.StringToHash("Roll Backward");
        public readonly int RollRightHash = Animator.StringToHash("Roll Right");
        public readonly int RollLeftHash = Animator.StringToHash("Roll Left");
        public readonly int IdleToJumpHash = Animator.StringToHash("Idle Jump");
        public readonly int FallingLoopHash = Animator.StringToHash("Falling Loop");
        public readonly int LandingHash = Animator.StringToHash("Falling To Landing");
        public readonly int LandingHeavyHash = Animator.StringToHash("Falling To Landing Heavy");
        public readonly int IdleToBracedHangingHash = Animator.StringToHash("Idle To Braced Hang");
        public readonly int BracedHangingHash = Animator.StringToHash("Braced Hanging Idle");
        public readonly int BracedHangToCrouchClimbHash = Animator.StringToHash("Braced Hang To Crouch");
        public readonly int JumpToFreeHandHangingHash = Animator.StringToHash("Jump To Freehang");
        public readonly int FreeHangingIdleHash = Animator.StringToHash("Free Hanging Idle");
        public readonly int FreeHangClimbHash = Animator.StringToHash("FreeHang Climb");
        public readonly int Mirror = Animator.StringToHash("Mirror");

        public int BlockingLayerIndex { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        private PlayerGroundedState CurrentGrounded => CurrentState as PlayerGroundedState;
        public State CurrentSubState => (CurrentState as State)?.GetSubState();

        [Space(10)] [Header("Ground Check Settings")] [Tooltip("Grounded Grace Period")] [SerializeField]
        private float groundedGrace = 0.1f;

        public float GroundedGrace => groundedGrace;

        public bool IsGrounded => GroundChecker != null && GroundChecker.IsGrounded;

        [Tooltip("If ground is this close below, we stay in GroundedState (useful for stairs/step-down).")]
        [SerializeField]
        private float groundedSnapDistance = 0.2f;

        public float GroundedSnapDistance => groundedSnapDistance;

        private void Awake()
        {
            BlockingLayerIndex = Animator.GetLayerIndex("Block Layer");
        }

        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDeath += HandleDeath;
        }


        private void Start()
        {
            if (UnityEngine.Camera.main == null) Debug.LogError("No main camera found!");
            if (UnityEngine.Camera.main != null) MainCameraTransform = UnityEngine.Camera.main.transform;

            SwitchState(new PlayerGroundedState(this));
        }


        private void HandleTakeDamage()
        {
            //Override States dont use switchSubstate

            // CurrentGrounded.SwitchSubState(new PlayerImpactState(this));

            SwitchState(new PlayerImpactState(this));
        }

        private void HandleDeath()
        {
            //Override States dont use switchSubstate
            // CurrentGrounded.SwitchSubState(new PlayerDeadState(this));

            SwitchState(new PlayerDeadState(this));
        }

        public void SetDodgeCooldownTime(float time)
        {
            previousDodgeTime = time;
        }

        public void SetRollCooldownTime(float time)
        {
            previousRollTime = time;
        }

        public void PendingJumpState(float time)
        {
            PendingJump = true;
            previousJumpTime = time;
        }

        public bool ConsumeJump() // if Jumping turn true
        {
            if (!PendingJump) return false;
            PendingJump = false;
            return true;
        }


        public void EquipWeapon(WeaponLogic.WeaponLogic newWeapon)
        {
            WeaponLogic = newWeapon;
            if (WeaponLogic != null)
                WeaponLogic.Initialize(Controller);
        }


        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDeath -= HandleDeath;
        }
    }
}