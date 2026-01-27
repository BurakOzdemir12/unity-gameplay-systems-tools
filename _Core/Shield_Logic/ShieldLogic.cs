using System;
using System.Collections.Generic;
using _Project.Systems._Core.Health.Interfaces;
using _Project.Systems._Core.Shield_Logic.Interfaces;
using _Project.Systems._Core.Shield_Logic.ScriptableObjects;
using _Project.Systems._Core.Shield_Logic.Structs;
using _Project.Systems._Core.WeaponLogic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.Shield_Logic
{
    public class ShieldLogic : MonoBehaviour, IBlocker
    {
        [Header("Shield Data")] [SerializeField]
        private ShieldDataSo shieldData;

        public ShieldDataSo ShieldData => shieldData;

        [Header("Character References")] [SerializeField]
        private Transform characterRoot;

        [Header("Collider References (Set in Inspector")] [SerializeField]
        private Collider characterOwnCollider;

        [SerializeField] private float frontDotThreshold = 0.2f;

        private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();

        private float currentShieldDamage;
        private float currentShieldDurability;
        private float currentShieldStunPower;

        public event Action<BlockContext> OnBlocked;
        public event Action<BlockContext> OnParried;
        public event Action OnShieldBreak;
        public bool IsBlocking { get; private set; }

        public bool ParryWindowActive { get; private set; }
        public void SetParryWindow(bool active) => ParryWindowActive = active;

        private void Awake()
        {
            if (characterRoot == null)
            {
                characterRoot = this.transform.root;
            }

            if (characterOwnCollider == null)
            {
                characterOwnCollider = characterRoot.GetComponent<Collider>();
            }
        }

        private void OnEnable()
        {
            hitColliders.Clear();
        }

        private void OnDisable()
        {
            hitColliders.Clear();
        }

        private void Start()
        {
            SetupShieldProps();
        }

        private void SetupShieldProps()
        {
            currentShieldDamage = shieldData.shieldDamage;
            currentShieldDurability = shieldData.shieldDurability;
            currentShieldStunPower = shieldData.shieldStunPower;
        }

        public void PerformBlock()
        {
            hitColliders.Clear();
            gameObject.SetActive(true);
            IsBlocking = true;
        }

        public void EndBlock()
        {
            hitColliders.Clear();
            gameObject.SetActive(false);
            IsBlocking = false;
            ParryWindowActive = false;
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == characterOwnCollider) return;
            if (!hitColliders.Add(other)) return;
        }

        public bool CanBlock(Transform attackerRoot)
        {
            Vector3 toAttacker = (attackerRoot.position - characterRoot.position).normalized;
            float dot = Vector3.Dot(characterRoot.forward, toAttacker);
            return dot > frontDotThreshold;
        }

        public void ApplyBlock(BlockContext context)
        {
            if (ParryWindowActive && IsBlocking && CanBlock(context.AttackerRoot))
            {
                OnParried?.Invoke(context);
                return;
            }

            OnBlocked?.Invoke(context);
            currentShieldDurability -= context.Damage;

            if (currentShieldDurability <= 0)
            {
                ShieldBreak();
                return;
            }
        }

        private void ShieldBreak()
        {
            OnShieldBreak?.Invoke();
            IsBlocking = false;
            ParryWindowActive = false;
        }
    }
}