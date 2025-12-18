using _Project.Core.Scripts;
using _Project.Systems.CombatAndTraversalSystem.Player.Combat;
using _Project.Systems.CombatAndTraversalSystem.Targeting;
using _Project.Systems.Core.GravityForce;
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
        [field: SerializeField] public AttackDataSo[] Attacks { get; private set; }

        // [Header("Weapon Transforms")] [Tooltip("Sword Holder Transform")] [field: SerializeField]
        // public Transform swordHolderR;

        [Header("Animation")]
        [Tooltip("The duration of the crossfade between the two blend trees")]
        [field: SerializeField]
        public float crossFadeDurationBetweenBlendTrees = 0.1f;

        [Tooltip(" The damp time of the animator parameters")] [field: SerializeField]
        public float locomotionAnimatorDampTime = 0.1f;

        [Tooltip(" The damp time of the animator parameters")] [field: SerializeField]
        public float targetingAnimatorDampTime = 0.2f;

        [Header("Movement")]
        [Tooltip(" The time it takes to rotate towards movement direction")]
        [field: SerializeField]
        public float rotationDampTime = 0.1f;

        [Tooltip(" The time it takes to rotate towards movement direction")] [field: SerializeField]
        public float rotationDampTimeWhileAttack = 0.1f;

        [Tooltip(" The speed at which the player moves when in free look mode")]
        [field: SerializeField]
        public float FreeMovementSpeed { get; private set; }

        [Tooltip(" The speed at which the player moves when in targeting mode")]
        [field: SerializeField]
        public float TargetingMovementSpeed { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        [Header("Attack settings")] [Tooltip(" Attack damage")] [field: SerializeField]
        public float attackDamage = 20f;


        public readonly int FreeLookSpeedParam = Animator.StringToHash("FreeLookSpeed");
        public readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
        public readonly int TargetingBlendTreeHash = Animator.StringToHash("TargetingBlendTree");
        public readonly int TargetingForwardSpeedHash = Animator.StringToHash("TargetingForwardSpeed");
        public readonly int TargetingRightSpeedHash = Animator.StringToHash("TargetingRightSpeed");

        private void Start()
        {
            if (UnityEngine.Camera.main == null) Debug.LogError("No main camera found!");
            if (UnityEngine.Camera.main != null) MainCameraTransform = UnityEngine.Camera.main.transform;
            SwitchState(new PlayerFreeLookState(this));

          
        }

        public void EquipWeapon(WeaponLogic newWeapon)
        {
            WeaponLogic = newWeapon;
            if (WeaponLogic != null)
                WeaponLogic.Initialize(Controller);
        }
    }
}