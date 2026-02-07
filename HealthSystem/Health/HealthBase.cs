using System;
using _Project.Systems.HealthSystem.Stun.Interfaces;
using UnityEngine;

namespace _Project.Systems.HealthSystem.Health
{
    public abstract class HealthBase : MonoBehaviour,IStunnable
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;
        public event Action OnDeath;
        public event Action OnTakeDamage;
        public event Action<float> OnStunned;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ApplyDamage(float damage)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth <= 0)
            {
                HandleCharacterDeath();
                return;
            }

            OnTakeDamage?.Invoke();
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