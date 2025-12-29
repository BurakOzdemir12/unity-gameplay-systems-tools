using System;
using _Project.Core.Scripts;
using _Project.Systems.CombatAndTraversalSystem.LedgeClimbing;
using _Project.Systems.CombatAndTraversalSystem.Player.Combat;
using _Project.Systems.CombatAndTraversalSystem.Player.Enums;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates;
using _Project.Systems.CombatAndTraversalSystem.Targeting;
using _Project.Systems.Core.GravityForce;
using _Project.Systems.Core.Health;
using _Project.Systems.Core.Ragdoll;
using _Project.Systems.Core.StateMachine;
using _Project.Systems.Core.WeaponLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerStateMachine : StateMachine
    {
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponLogic WeaponLogic { get; private set; }
        [field: SerializeField] public PlayerHealth Health { get; private set; }
        [field: SerializeField] public Ragdoll Ragdoll { get; private set; }
        [field: SerializeField] public LedgeValidator LedgeValidator { get; private set; }
        [field: SerializeField] public AttackDataSo[] Attacks { get; private set; }

        // [Header("Weapon Transforms")] [Tooltip("Sword Holder Transform")] [field: SerializeField]
        // public Transform swordHolderR;

        [Header("Animation")] [Tooltip("The duration of the crossfade between the two blend trees")] [SerializeField]
        private float crossFadeDurationBetweenBlendTrees = 0.1f;

        public float CrossFadeDurationBetweenBlendTrees => crossFadeDurationBetweenBlendTrees;

        [Tooltip(" The damp time of the animator parameters")] [SerializeField]
        private float locomotionAnimatorDampTime = 0.1f;

        public float LocomotionAnimatorDampTime => locomotionAnimatorDampTime;

        [Tooltip(" The damp time of the animator parameters")] [SerializeField]
        private float targetingAnimatorDampTime = 0.2f;

        public float TargetingAnimatorDampTime => targetingAnimatorDampTime;

        [Header("Movement")] [Tooltip(" The time it takes to rotate towards movement direction")] [SerializeField]
        private float rotationDampTime = 0.1f;

        public float RotationDampTime => rotationDampTime;

        [Tooltip(" The speed at which the player moves when in free look mode")] [SerializeField]
        private float freeMovementSpeed;

        public float FreeMovementSpeed => freeMovementSpeed;

        [Tooltip(" The speed at which the player moves when in targeting mode")] [SerializeField]
        public float targetingMovementSpeed;

        public float TargetingMovementSpeed => targetingMovementSpeed;


        [Header("Attack settings")] [Tooltip(" Attack damage")] [SerializeField]
        private float attackDamage = 20f;

        public float AttackDamage => attackDamage;

        [Tooltip(" The time it takes to rotate towards movement direction")] [SerializeField]
        private float rotationDampTimeWhileAttack = 0.1f;

        public float RotationDampTimeWhileAttack => rotationDampTimeWhileAttack;

        [Header("Impact Settings")] [Tooltip(" Impact duration")] [SerializeField]
        private float impactDuration = 0.1f;

        public float ImpactDuration => impactDuration;

        [Header("Animation Settings")] [Tooltip("The duration time of the locomotion blend tree ")] [SerializeField]
        private float crossFadeDuration = 0.1f;

        public float CrossFadeDuration => crossFadeDuration;

        [Header("Blocking Settings")] [Tooltip("The Blocking layer Change speed ")] [SerializeField]
        private float blockingLayerChangeSpeed = 0.1f;

        public float BlockingLayerChangeSpeed => blockingLayerChangeSpeed;

        [field: SerializeField] public float blockLayerWeight = 1;

        [Tooltip(" The time it takes to rotate towards movement direction when Block")] [SerializeField]
        private float rotationDampTimeWhileBlock = 2f;

        public float RotationDampTimeWhileBlock => rotationDampTimeWhileBlock;

        [Header("Dodge Settings")] [Tooltip("Dodge speed")] [SerializeField]
        private float dodgeSpeed = 10f;

        public float DodgeSpeed => dodgeSpeed;

        [Tooltip(" The time it takes to rotate towards movement direction")] [SerializeField]
        private float rotationDampTimeWhileDodge = 2f;

        public float RotationDampTimeWhileDodge => rotationDampTimeWhileDodge;


        [Tooltip("Dodge Press Previous Time ")]
        private float previousDodgeTime;

        public float PreviousDodgeTime
        {
            get => previousDodgeTime;
            set => previousDodgeTime = value;
        }

        [Tooltip("Dodge Cooldown Time")] [SerializeField]
        private float dodgeCooldownTime = 2f;

        public float DodgeCooldownTime => dodgeCooldownTime;


        [Tooltip("Dodge Animation Start Time")] [Range(0f, 1f)] [SerializeField]
        private float dodgeAnimStartTime;

        public float DodgeAnimStartTime => dodgeAnimStartTime;

        [Tooltip("Dodge Animation End Time")] [Range(0f, 1f)] [SerializeField]
        private float dodgeAnimEndTime;

        public float DodgeAnimEndTime => dodgeAnimEndTime;

        [Header("Roll Settings")] [Tooltip("Roll Speed")] [SerializeField]
        private float rollSpeed = 2f;

        public float RollSpeed => rollSpeed;

        [Tooltip(" The time it takes to rotate towards movement direction while Roll")] [SerializeField]
        private float rotationDampTimeWhileRoll = 2f;

        public float RotationDampTimeWhileRoll => rotationDampTimeWhileRoll;

        [Tooltip("Roll Cooldown")] [SerializeField]
        private float rollCooldownTime = 2f;

        public float RollCooldownTime => rollCooldownTime;

        [Tooltip("Previous time of roll animation")]
        private float previousRollTime;

        public float PreviousRollTime
        {
            get => previousRollTime;
            set => previousRollTime = value;
        }

        [Tooltip("Roll Animation Start Time")] [Range(0f, 1f)] [SerializeField]
        private float rollAnimStartTime;

        public float RollAnimStartTime => rollAnimStartTime;

        [Tooltip("Roll Animation End Time")] [Range(0f, 1f)] [SerializeField]
        private float rollAnimEndTime;

        public float RollAnimEndTime => rollAnimEndTime;

        [Header("Jump Settings")] [Tooltip("Jump Force")] [SerializeField]
        private float jumpForce = 10f;

        public float JumpForce => jumpForce;

        [Tooltip("Jump Cooldown")] [SerializeField]
        private float jumpCooldownTime = 2f;

        public float JumpCooldownTime => jumpCooldownTime;

        [field: Tooltip("Previous time of jump animation")]
        public bool PendingJump { get; private set; }

        private float previousJumpTime;

        public float PreviousJumpTime
        {
            get => previousJumpTime;
            set => previousJumpTime = value;
        }

        [Header("Falling - Landing Settings")] [Tooltip("Falling State Start Velocity")] [SerializeField]
        private float fallingVelocityThreshold;

        public float FallingVelocityThreshold => fallingVelocityThreshold;

        [Tooltip("Falling State Start Heightens")] [SerializeField]
        private float airborneHeightThreshold;

        public float AirborneHeightThreshold => airborneHeightThreshold;

        [Tooltip("Landing State Start Heightens")] [SerializeField]
        private float landingHeightThreshold;

        public float LandingHeightThreshold => landingHeightThreshold;

        [Tooltip("Hard Landing  Start Heightens")] [SerializeField]
        private float landingHardHeightThreshold;

        public float LandingHardHeightThreshold => landingHardHeightThreshold;

        [Tooltip("Landing Animation Start Time")] [Range(0f, 2f)] [SerializeField]
        private float landingStateExitTime;

        public float LandingStateExitTime => landingStateExitTime;

        [Header("Ledge Climbing")] [Tooltip("Auto Climb Max Height")] [SerializeField]
        private float autoClimbMaxHeight;

        public float AutoClimbMaxHeight => autoClimbMaxHeight;

        [Tooltip("is Climbing Free Flow")]
        [field: SerializeField]
        public bool IsFreeFlowClimb { get; private set; }

        // [Tooltip("Landing Animation Start Time")] [Range(0f, 2f)] [SerializeField]
        // private float landingAnimStartTime;
        //
        // public float LandingAnimStartTime => landingAnimStartTime;
        //
        // [Tooltip("Landing Animation End Time")] [Range(0f, 2f)] [SerializeField]
        // private float landingAnimEndTime;
        //
        // public float LandingAnimEndTime => landingAnimEndTime;

        [Tooltip("İf Your animations works with the root motion, set this to true")] [SerializeField]
        public bool workWithRootMotion = false;

        [Tooltip("In Combat or alert mode, its gonna roll but in normal mode jump will work ")] [SerializeField]
        private bool inAlertMode = false;

        public bool IsInAlertMode => inAlertMode;

        // TODO Create scriptable object or seperated static class for stock animator hashes

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

        public int BlockingLayerIndex { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        private PlayerGroundedState CurrentGrounded => CurrentState as PlayerGroundedState;
        public State CurrentSubState => (CurrentState as State)?.GetSubState();

        [SerializeField] private float groundedGrace = 0.1f;
        public float GroundedGrace => groundedGrace;

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

        // public void DecideTargetOrLocomotion()
        // {
        //     if (Targeter.SelectedTarget != null)
        //     {
        //         
        //         CurrentGrounded.SwitchSubState(new PlayerTargetingState(this));
        //         // SwitchState(new PlayerTargetingState(this));
        //     }
        //     else
        //     {
        //         CurrentGrounded.SwitchSubState(new PlayerFreeLookState(this));
        //
        //         // SwitchState(new PlayerFreeLookState(this));
        //     }
        // }

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


        public void EquipWeapon(WeaponLogic newWeapon)
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