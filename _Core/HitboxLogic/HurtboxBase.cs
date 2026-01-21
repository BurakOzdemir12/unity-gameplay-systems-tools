using System;
using _Project.Systems._Core.Health;
using _Project.Systems._Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.HitboxLogic
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