using System;
using _Project.Systems.Core.GravityForce;
using _Project.Systems.Core.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace _Project.Systems.CombatAndTraversalSystem.Enemy.States
{
    public class EnemyStateMachine : StateMachine
    {
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }

        [Header("Movement Settings")]
        [Tooltip("Move Speed")]
        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        [Tooltip("Rotation Damp Time")]
        [field: SerializeField]
        public float RotationDampTime { get; private set; }

        [Header("Chase Settings")]
        [Tooltip("Chase Range")]
        [field: SerializeField]
        public float ChaseDetectionRange { get; private set; }

        [Tooltip("Chase and Attack detect buffer length")] [field: SerializeField]
        public int bufferMax = 4;

        [Tooltip("Chase Detection Layers")]
        [field: SerializeField]
        public LayerMask ChaseDetectionLayers { get; private set; }

        [Header("Attack Settings")]
        [Tooltip("Attack Range")]
        [field: SerializeField]
        public float AttackRange { get; private set; }

        [Tooltip("Attack Detection Layers")]
        [field: SerializeField]
        public LayerMask AttackDetectionLayers { get; private set; }


        [Header("Animation Settings")]
        [Tooltip("The duration time of the locomotion blend tree ")]
        [field: SerializeField]
        public float CrossFadeDuration { get; private set; } = 0.1f;

        [Tooltip("The duration time for the Combat blend tree ")]
        [field: SerializeField]
        public float CrossFadeDurationCombat { get; private set; } = 0.1f;

        [Tooltip(" The damp time of the animator parameters")] [field: SerializeField]
        public float locomotionAnimationDampTime;

        public GameObject Player { get; set; }
        public Collider[] buffersForChase;
        public Collider[] buffersForAttack;

        // public readonly int AttackBlendTreeHash = Animator.StringToHash("CombatBlendTree");
        public readonly int CombatIdleHash = Animator.StringToHash("EnemyCombatIdle");
        public readonly int EnemyAttack1RHash = Animator.StringToHash("EnemyAttack1R");
        public readonly int MoveSpeedParamHash = Animator.StringToHash("MoveSpeed");
        public readonly int LocomotionBlendTreeHash = Animator.StringToHash("Locomotion");

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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ChaseDetectionRange);

            Gizmos.color = Color.darkRed;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
    }
}