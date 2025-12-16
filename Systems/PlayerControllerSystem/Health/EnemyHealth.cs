using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.Health
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
            if (currentHealth <= 0) return;
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            Debug.Log("Enemy Get Hit =>" + damage + "Now Health=>" + currentHealth);
        }
    }
}