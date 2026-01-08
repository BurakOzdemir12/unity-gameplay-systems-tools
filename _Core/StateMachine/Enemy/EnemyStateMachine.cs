using _Project.Systems._Core.GravityForce;
using _Project.Systems._Core.Health;
using _Project.Systems._Core.ScriptableObjects.Characters;
using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.CombatSystem.Targeting;
using _Project.Systems.MovementSystem.Enemy.States;
using UnityEngine;
using UnityEngine.AI;
using StateMachine_StateMachine = _Project.Systems._Core.StateMachine.StateMachine;

namespace _Project.Systems._Core.StateMachine.Enemy
{
    public class EnemyStateMachine : StateMachine_StateMachine
    {
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponLogic.WeaponLogic WeaponLogic { get; private set; }
        [field: SerializeField] public EnemyHealth Health { get; private set; }
        [field: SerializeField] public Target Target { get; private set; }
        [field: SerializeField] public Ragdoll.Ragdoll Ragdoll { get; private set; }
        [field: SerializeField] public EnemyConfigSo EnemyConfigSo { get; private set; }

        [Tooltip("Chase and Attack detect buffer length")] [SerializeField]
        private int bufferMax = 4;

        public int BufferMax => bufferMax;

        public GameObject Player { get; set; }
        public Collider[] buffersForChase;
        public Collider[] buffersForAttack;

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
            Agent.updatePosition = false;
            Agent.updateRotation = false;

            Controller = GetComponent<CharacterController>();

            buffersForChase = new Collider[bufferMax];
            buffersForAttack = new Collider[bufferMax];
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

        public void EquipWeapon(WeaponLogic.WeaponLogic newWeapon)
        {
            WeaponLogic = newWeapon;
            if (WeaponLogic != null)
                WeaponLogic.Initialize(Controller);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position,EnemyConfigSo.MovementData.ChaseDetectionRange);

            Gizmos.color = Color.darkRed;
            Gizmos.DrawWireSphere(transform.position,EnemyConfigSo.CombatData.AttackRange);
        }

        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDeath -= HandleDeath;
        }
    }
}