using System.Collections.Generic;
using _Project.Systems._Core.BaseScriptableObjects.Characters;
using _Project.Systems._Core.Field_of_View;
using _Project.Systems._Core.GravityForce;
using _Project.Systems._Core.Health;
using _Project.Systems._Core.Weapon_Tool_Handlers;
using _Project.Systems.CombatSystem.Enemy;
using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.CombatSystem.Targeting;
using _Project.Systems.MovementSystem.Enemy.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Systems._Core.StateMachine.Enemy
{
    public class EnemyStateMachine : StateMachine
    {
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponHandler WeaponHandler { get; private set; }
        [field: SerializeField] public ShieldHandler ShieldHandler { get; private set; }
        [field: SerializeField] public EnemyHealth Health { get; private set; }
        [field: SerializeField] public Target Target { get; private set; }
        [field: SerializeField] public Ragdoll.Ragdoll Ragdoll { get; private set; }
        [field: SerializeField] public EnemyConfigSo EnemyConfigSo { get; private set; }
        [field: SerializeField] public FieldOfView FieldOfView { get; private set; }
        [field: SerializeField] public EnemyDefenceBrain EnemyDefenceBrain { get; private set; }

        [Tooltip("Chase and Attack detect buffer length")] [SerializeField]
        private int bufferMax = 4;

        public int BufferMax => bufferMax;

        [Header("Blocking Settings")] [field: SerializeField]
        public float blockLayerWeight = 0;

        public GameObject Player { get; set; }

        public HashSet<Collider> BuffersForChase;
        public HashSet<Collider> BuffersForAttack;
        public List<Collider> debugBuffersForChase;
        public List<Collider> debugBuffersForAttack;

        public Vector3 firstSpawnPoint;
        public int BlockingLayerIndex { get; private set; }
        
        // public readonly int AttackBlendTreeHash = Animator.StringToHash("CombatBlendTree");
        public readonly int CombatIdleHash = Animator.StringToHash("EnemyCombatIdle");
        public readonly int EnemyAttack1RHash = Animator.StringToHash("EnemyAttack1R");
        public readonly int MoveSpeedParamHash = Animator.StringToHash("MoveSpeed");
        public readonly int LocomotionBlendTreeHash = Animator.StringToHash("Locomotion");
        public readonly int LightImpactHash = Animator.StringToHash("ImpactSlight");

        //TODO Use animation override controller or create AnimationProfileSo for Hashes 

        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDeath += HandleDeath;
        }

        private void Start()
        {
            firstSpawnPoint = transform.position;
            Controller = GetComponent<CharacterController>();

            BuffersForChase = new HashSet<Collider>(bufferMax);
            BuffersForAttack = new HashSet<Collider>(bufferMax);

            BlockingLayerIndex = Animator.GetLayerIndex("Block Layer");

            SwitchState(new EnemyIdleState(this));
        }

        private void HandleTakeDamage()
        {
            SwitchState(new EnemyImpactState(this));
        }

        private void HandleDeath()
        {
            SwitchState(new EnemyDeadState(this));
        }

        private void OnDrawGizmosSelected()
        {
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawWireSphere(transform.position, EnemyConfigSo.MovementData.ChaseDetectionRange);

            Gizmos.color = Color.darkRed;
            Gizmos.DrawWireSphere(transform.position + EnemyConfigSo.CombatData.AttackPositionOffset,
                EnemyConfigSo.CombatData.AttackRange);

            // Gizmos.color = Color.black;
            // for (int i = 0; i < waypoints.Length; i++)
            // {
            //     Gizmos.DrawWireSphere(waypoints[i], 0.35f);
            //     if (i + 1 < waypoints.Length)
            //     {
            //         Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            //     }
            //     else
            //     {
            //         Gizmos.DrawLine(waypoints[i], waypoints[0]);
            //     }
            // }
        }

        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDeath -= HandleDeath;
        }
    }
}