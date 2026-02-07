using _Project.Systems.HealthSystem.Health;
using _Project.Systems.HealthSystem.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems.HealthSystem.HurtboxLogic
{
    public abstract class HurtboxBase : MonoBehaviour, IDamageable
    {

        protected HealthBase OwnerHealth;

        protected virtual void Awake()
        {
            if (!OwnerHealth)
                OwnerHealth = GetComponentInParent<HealthBase>();
        }

        public virtual void ApplyDamage(float damage)
        {
            OwnerHealth.ApplyDamage(damage);
        }
        protected abstract void OnHitApplied(float finalDamage);
    }
}