using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using _Project.Systems._Core.GravityForce;
using _Project.Systems.CombatSystem.Enemy;
using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.CombatSystem.Targeting;
using _Project.Systems.EnemyPerceptionSystem;
using _Project.Systems.EnemyPerceptionSystem.Field_of_View;
using _Project.Systems.HealthSystem.Health;
using _Project.Systems.HealthSystem.Ragdoll;
using _Project.Systems.HealthSystem.Structs;
using _Project.Systems.MovementSystem.Enemy.States;
using _Project.Systems.SharedGameplay.BaseScriptableObjects.Characters;
using _Project.Systems.SharedGameplay.Shield_Logic.Structs;
using _Project.Systems.SharedGameplay.Weapon_Tool_Handlers;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Systems.SharedGameplay.StateMachine.Enemy
{
    public class EnemyStateMachine : _Core.StateMachine.StateMachine
    {
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [field: SerializeField] public WeaponHandler WeaponHandler { get; private set; }
        [field: SerializeField] public ShieldHandler ShieldHandler { get; private set; }
        [field: SerializeField] public EnemyHealth Health { get; private set; }
        [field: SerializeField] public Target Target { get; private set; }
        [field: SerializeField] public Ragdoll Ragdoll { get; private set; }
        [field: SerializeField] public EnemyConfigSo EnemyConfigSo { get; private set; }
        [field: SerializeField] public FieldOfView FieldOfView { get; private set; }
        [field: SerializeField] public EnemyDefenceBrain EnemyDefenceBrain { get; private set; }
        [field: SerializeField] public EnemyPerceptionController EnemyPerceptionController { get; private set; }

        [Tooltip("Chase and Attack detect buffer length")] [SerializeField]
        private int bufferMax = 4;

        public int BufferMax => bufferMax;

        [Header("Blocking Settings")] [field: SerializeField]
        public float blockLayerWeight = 0;


        public Vector3 firstSpawnPoint;
        public int BlockingLayerIndex { get; private set; }

        private void Awake()
        {
            BlockingLayerIndex = Animator.GetLayerIndex("Block Layer");
        }

        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDeath += HandleDeath;
            Health.OnStunned += HandleStunned;
            ShieldHandler.CurrentShieldLogic.OnBlocked += HandleShieldImpact;
        }

        private void Start()
        {
            firstSpawnPoint = transform.position;
            Controller = GetComponent<CharacterController>();
            //EnemyPerception Init
            if (EnemyPerceptionController == null)
            {
                EnemyPerceptionController = GetComponent<EnemyPerceptionController>();
            }

            EnemyPerceptionController.Initialize(gameObject, EnemyConfigSo, FieldOfView);

            SwitchState(new EnemyIdleState(this));
        }

        private void HandleTakeDamage(DamageInfo damageInfo)
        {
            SwitchState(new EnemyImpactState(this));
            if (damageInfo.SourceObject != null)
            {
                EnemyPerceptionController.OnDamageTaken(damageInfo.SourceObject);
            }
        }

        private void HandleDeath()
        {
            EnemyPerceptionController.OnDeath();
            SwitchState(new EnemyDeadState(this));
        }

        private void HandleShieldImpact(BlockContext ctx)
        {
            Animator.CrossFadeInFixedTime(EnemyConfigSo.CombatData.BlockImpactAnimHash,
                EnemyConfigSo.CombatData.CrossFadeDurationCombat);
            // SwitchState(new EnemyBlockParryState(this, ctx));
        }

        private void HandleStunned(float duration)
        {
            SwitchState(new EnemyStunnedState(this, duration));
        }

        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDeath -= HandleDeath;
            Health.OnStunned -= HandleStunned;
            ShieldHandler.CurrentShieldLogic.OnBlocked -= HandleShieldImpact;
        }
    }
}