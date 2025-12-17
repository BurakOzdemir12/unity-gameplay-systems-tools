using System.Collections.Generic;
using _Project.Systems.Core.Health;
using _Project.Systems.Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems.Core.WeaponLogic
{
    public class WeaponLogic : MonoBehaviour
    {
        [SerializeField] private Collider playerOwnCollider;
        [SerializeField] private float attackDamage;
        private readonly HashSet<Collider> damagedColliders = new HashSet<Collider>();

        private float currentAttackMultiplier = 1f;

        private void OnEnable()
        {
            damagedColliders.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == playerOwnCollider) return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable == null) return;

                if (!damagedColliders.Add(other)) return;

                float finalDamage = attackDamage * currentAttackMultiplier;
                damageable.ApplyDamage(finalDamage);
                Debug.Log("damaged");
            }
        }

        public void SetAttackAttributes(float damageMultiplier)
        {
            currentAttackMultiplier = damageMultiplier;
        }
    }
}