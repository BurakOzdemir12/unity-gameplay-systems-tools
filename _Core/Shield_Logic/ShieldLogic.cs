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
        [Header("Weapon Data")] [SerializeField]
        private GameObject shieldModel;

        [Header("Weapon Data")] [SerializeField]
        private ShieldDataSo shieldData;

        public ShieldDataSo ShieldData => shieldData;

        [SerializeField] private Transform enemyRoot;
        [SerializeField] private float frontDotThreshold = 0.2f;

        [Header("Collider References (Set in Inspector")] [SerializeField]
        private Collider characterOwnCollider;

        private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();

        private float currentShieldDamage;
        private float currentShieldDurability;
        private float currentShieldStunPower;

        public void Initialize(Collider ownerCollider) => characterOwnCollider = ownerCollider;
        public bool IsBlocking { get; private set; }

        private void OnEnable()
        {
            hitColliders.Clear();
        }

        private void OnDisable()
        {
            hitColliders.Clear();
        }

        public void SetupDefence()
        {
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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == characterOwnCollider) return;
            if (!hitColliders.Add(other)) return;

            // ApplyStun(other);
        }

        public bool CanBlock(Transform attackerRoot)
        {
            Vector3 toAttacker = (attackerRoot.position - enemyRoot.position).normalized;
            float dot = Vector3.Dot(enemyRoot.forward, toAttacker);
            return dot > frontDotThreshold;
        }

        public void ApplyBlock(BlockContext context)
        {
            Debug.Log("Applying Block");
        }
    }
}