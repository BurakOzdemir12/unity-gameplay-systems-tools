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

        //TODO Implement damage
        void Start()
        {
        }

        void Update()
        {
        }

        public void ApplyDamage(float damage)
        {
            throw new NotImplementedException();
        }
    }
}