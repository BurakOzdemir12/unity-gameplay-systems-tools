using System.Collections.Generic;
using _Project.Systems._Core.GravityForce.Interfaces;
using _Project.Systems._Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.WeaponLogic
{
    public class WeaponLogic : MonoBehaviour
    {
        [SerializeField] private Collider characterOwnCollider;
        private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();
        
        private bool active;
        private float currentDamage;
        private float currentKnockbackForce;

        private float currentAttackMultiplier = 1f;

        private void OnEnable()
        {
            hitColliders.Clear();
        }

        private void OnDisable()
        {
            hitColliders.Clear();
            active = false;
        }

        public void Initialize(Collider ownerCollider)
        {
            characterOwnCollider = ownerCollider;
        }

        public void BeginAttack(float finalDamage, float finalKnockbackForce)
        {
            currentDamage = finalDamage;
            currentKnockbackForce = finalKnockbackForce;
            
            hitColliders.Clear();
            active = true;
            
            gameObject.SetActive(true);
        }

        public void EndAttack()
        {
            active = false;
            hitColliders.Clear();

            gameObject.SetActive(false);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!active) return;

            if (other == characterOwnCollider) return;
            if (!hitColliders.Add(other)) return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable == null) return;


                damageable.ApplyDamage(currentDamage);
            }

            if (other.TryGetComponent<IKnockable>(out var knockable))
            {
                if (knockable == null) return;
                Vector3 dir = (other.transform.position - transform.root.position);
                // Vector3 dir = (other.transform.position - characterOwnCollider.transform.position);
                dir.y = 0f;
                knockable.ApplyKnockback(currentKnockbackForce, dir);
            }
        }

        // public void SetAttackAttributes(float damageMultiplier, float knockbackForce, float attackDamage)
        // {
        //     this.currentAttackMultiplier = damageMultiplier;
        //     this.currentKnockbackForce = knockbackForce;
        //     this.currentDamage = attackDamage;
        // }
    }
}