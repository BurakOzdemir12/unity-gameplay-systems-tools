using System;
using _Project.Systems.Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems.Core.Health
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;

        public event Action OnTakeDamage;
        public event Action OnDeath;

        private void Awake()
        {
            currentHealth = maxHealth;
        }


        public void ApplyDamage(float damage)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            OnTakeDamage?.Invoke();
            if (currentHealth <= 0)
            {
                HandlePlayerDeath();
                return;
            }
        }

        private void HandlePlayerDeath()
        {
            OnDeath?.Invoke();
            // Destroy(this.gameObject);
        }
    }
}