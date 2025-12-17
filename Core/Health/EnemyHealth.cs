using _Project.Systems.Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems.Core.Health
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;

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

            Debug.Log("Enemy Get Hit =>" + damage + "Now Health=>" + currentHealth);
        }

        private void HandleEnemyDeath()
        {
            Destroy(this.gameObject);
        }
    }
}