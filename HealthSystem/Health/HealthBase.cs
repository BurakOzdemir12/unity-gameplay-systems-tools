using System;
using _Project.Systems.HealthSystem.Structs;
using _Project.Systems.HealthSystem.Stun.Interfaces;
using UnityEngine;

namespace _Project.Systems.HealthSystem.Health
{
    public abstract class HealthBase : MonoBehaviour, IStunnable
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;
        public event Action OnDeath;
        public event Action<DamageInfo> OnTakeDamage;
        public event Action<float> OnStunned;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ApplyDamage(DamageInfo damageInfo)
        {
            currentHealth = Mathf.Max(currentHealth - damageInfo.Damage, 0);
            OnTakeDamage?.Invoke(damageInfo);

            if (currentHealth <= 0)
            {
                HandleCharacterDeath();
            }
        }

        private void HandleCharacterDeath()
        {
            OnDeath?.Invoke();
            // Destroy(this.gameObject);
        }

        public void ApplyStun(float duration)
        {
            OnStunned?.Invoke(duration);
        }
        // protected abstract void OnPlayerDeath();
        // protected abstract void OnCharacterDamaged();
    }
}