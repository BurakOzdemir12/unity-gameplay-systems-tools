using System;
using _Project.Systems.Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems.Core.Health
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;

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
        }

        private void HandlePlayerDeath()
        {
            Destroy(this.gameObject);
        }
    }
}