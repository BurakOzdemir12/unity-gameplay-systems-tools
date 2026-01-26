using System;
using UnityEngine;

namespace _Project.Systems._Core.Health
{
    public abstract class HealthBase : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;
        public event Action OnDeath;
        public event Action OnTakeDamage;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ApplyDamage(float damage)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth <= 0)
            {
                HandlePlayerDeath();
                return;
            }

            OnTakeDamage?.Invoke();
        }

        private void HandlePlayerDeath()
        {
            OnDeath?.Invoke();
            // Destroy(this.gameObject);
        }

        // protected abstract void OnPlayerDeath();
        // protected abstract void OnCharacterDamaged();
    }
}