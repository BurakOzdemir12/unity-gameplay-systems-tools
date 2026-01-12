using System;
using _Project.Systems._Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.Health
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
    }
}