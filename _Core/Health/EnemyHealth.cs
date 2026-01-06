using System;
using _Project.Systems._Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.Health
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;
        public event Action OnTakeDamage;
        public event Action OnDeath;

        void Start()
        {
            currentHealth = maxHealth;
        }

        void Update()
        {
        }

        public void ApplyDamage(float damage)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);

            if (currentHealth <= 0)
            {
                HandleEnemyDeath();
                return;
            }
            OnTakeDamage?.Invoke();

            Debug.Log("Enemy Get Hit =>" + damage + "Now Health=>" + currentHealth);
        }

        private void HandleEnemyDeath()
        {
            OnDeath?.Invoke();
            // Destroy(this.gameObject);
        }
    }
}