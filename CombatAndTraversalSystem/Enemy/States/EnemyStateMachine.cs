using System;
using _Project.Systems.CombatAndTraversalSystem.Targeting;
using _Project.Systems.Core.GravityForce;
using _Project.Systems.Core.Health;
using _Project.Systems.Core.WeaponLogic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using StateMachine = _Project.Systems.Core.StateMachine.StateMachine;

namespace _Project.Systems.CombatAndTraversalSystem.Enemy.States
{
    public class EnemyStateMachine : StateMachine
    {
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponLogic WeaponLogic { get; private set; }
        [field: SerializeField] public EnemyHealth Health { get; private set; }
        [field: SerializeField] public Target Target { get; private set; }

        [Header("Movement")] [Tooltip("Move Speed")] [SerializeField]
        private float moveSpeed;

        public float MoveSpeed => moveSpeed;

        [Tooltip("Rotation Damp Time")] [SerializeField]
        private float rotationDampTime;

        public float RotationDampTime => rotationDampTime;

        [Header("Chase")] [Tooltip("Chase Range")] [SerializeField]
        private float chaseDetectionRange;

        public float ChaseDetectionRange => chaseDetectionRange;

        [Tooltip("Chase and Attack detect buffer length")] [SerializeField]
        private int bufferMax = 4;

        public int BufferMax => bufferMax;

        [Tooltip("Chase Detection Layers")]
        [field: SerializeField]
        public LayerMask ChaseDetectionLayers { get; private set; }

        [Header("Attack")] [Tooltip("Attack Range")] [SerializeField]
        private float attackRange;

        public float AttackRange => attackRange;

        [Tooltip("Attack Detection Layers")] [SerializeField]
        private LayerMask attackDetectionLayers;

        public LayerMask AttackDetectionLayers => attackDetectionLayers;

        [Tooltip("Attack Damage Value")] [SerializeField]
        private float attackDamage;

        public float AttackDamage => attackDamage;

        [Tooltip("Attack Cooldown Time")] [SerializeField]
        private float attackCoolDown;

        public float AttackCoolDown => attackCoolDown;

        [Tooltip("Attack Force Value")] [SerializeField]
        private float attackKnockBackForce;

        public float AttackKnockBackForce => attackKnockBackForce;

        [Tooltip("Attack Force Time")] [SerializeField]
        private float forceTime;

        public float ForceTime => forceTime;

        [Header("Animation")] [Tooltip("The duration time of the locomotion blend tree ")] [SerializeField]
        private float locomotionBlendTreeDuration = 0.1f;

        public float LocomotionBlendTreeDuration => locomotionBlendTreeDuration;

        [Tooltip("The duration time for the Combat blend tree ")] [SerializeField]
        private float crossFadeDurationCombat = 0.1f;

        public float CrossFadeDurationCombat => crossFadeDurationCombat;

        [Tooltip("The duration time for the Combat blend tree ")] [SerializeField]
        private float crossFadeDuration = 0.1f;

        public float CrossFadeDuration => crossFadeDuration;

        [Tooltip(" The damp time of the animator parameters")] [SerializeField]
        private float locomotionAnimationDampTime = 0.1f;

        public float LocomotionAnimationDampTime => locomotionAnimationDampTime;

        [Space(1)] [Header("Impact")] [Tooltip("Impact duration")] [SerializeField]
        private float impactDuration = 1f;

        public float ImpactDuration => impactDuration;

        public GameObject Player { get; set; }
        public Collider[] buffersForChase;
        public Collider[] buffersForAttack;

        // public readonly int AttackBlendTreeHash = Animator.StringToHash("CombatBlendTree");
        public readonly int CombatIdleHash = Animator.StringToHash("EnemyCombatIdle");
        public readonly int EnemyAttack1RHash = Animator.StringToHash("EnemyAttack1R");
        public readonly int MoveSpeedParamHash = Animator.StringToHash("MoveSpeed");
        public readonly int LocomotionBlendTreeHash = Animator.StringToHash("Locomotion");
        public readonly int LightImpactHash = Animator.StringToHash("ImpactSlight");

        //TODO Create AttackSo for Enemy or change your own AttackSo script for make suitable for enemy either 


        private void Start()
        {
            Agent.updatePosition = false;
            Agent.updateRotation = false;

            Controller = GetComponent<CharacterController>();

            buffersForChase = new Collider[bufferMax];
            buffersForAttack = new Collider[bufferMax];
            SwitchState(new EnemyIdleState(this));
        }
        public void EquipWeapon(WeaponLogic newWeapon)
        {
            WeaponLogic = newWeapon;
            if (WeaponLogic != null)
                WeaponLogic.Initialize(Controller);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ChaseDetectionRange);

            Gizmos.color = Color.darkRed;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
    }
}