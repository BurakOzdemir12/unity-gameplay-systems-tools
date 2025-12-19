using System;
using _Project.Core.Scripts;
using _Project.Systems.CombatAndTraversalSystem.Player.Combat;
using _Project.Systems.CombatAndTraversalSystem.Targeting;
using _Project.Systems.Core.GravityForce;
using _Project.Systems.Core.Health;
using _Project.Systems.Core.StateMachine;
using _Project.Systems.Core.WeaponLogic;
using UnityEngine;

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

        [Tooltip(" The time it takes to rotate towards movement direction")] [SerializeField]
        private float rotationDampTimeWhileAttack = 0.1f;

        public float RotationDampTimeWhileAttack => rotationDampTimeWhileAttack;

        [Tooltip(" The speed at which the player moves when in free look mode")] [SerializeField]
        private float freeMovementSpeed;

        public float FreeMovementSpeed => freeMovementSpeed;

        [Tooltip(" The speed at which the player moves when in targeting mode")] [SerializeField]
        public float targetingMovementSpeed;

        public float TargetingMovementSpeed => targetingMovementSpeed;


        [Header("Attack settings")] [Tooltip(" Attack damage")] [SerializeField]
        private float attackDamage = 20f;

        public float AttackDamage => attackDamage;

        [Header("Impact Settings")] [Tooltip(" Impact duration")] [SerializeField]
        private float impactDuration = 0.1f;

        public float ImpactDuration => impactDuration;

        [Header("Animation Settings")] [Tooltip("The duration time of the locomotion blend tree ")] [SerializeField]
        private float crossFadeDuration = 0.1f;

        public float CrossFadeDuration => crossFadeDuration;

        public readonly int FreeLookSpeedParam = Animator.StringToHash("FreeLookSpeed");
        public readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
        public readonly int TargetingBlendTreeHash = Animator.StringToHash("TargetingBlendTree");
        public readonly int TargetingForwardSpeedHash = Animator.StringToHash("TargetingForwardSpeed");
        public readonly int TargetingRightSpeedHash = Animator.StringToHash("TargetingRightSpeed");
        public readonly int LightImpactHash = Animator.StringToHash("ImpactSlight");

        public Transform MainCameraTransform { get; private set; }

        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDeath += HandleDeath;
        }


        private void Start()
        {
            if (UnityEngine.Camera.main == null) Debug.LogError("No main camera found!");
            if (UnityEngine.Camera.main != null) MainCameraTransform = UnityEngine.Camera.main.transform;
            SwitchState(new PlayerFreeLookState(this));
        }

        private void HandleTakeDamage()
        {
            SwitchState(new PlayerImpactState(this));
        }

        private void HandleDeath()
        {
            SwitchState(new PlayerDeadState(this));
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