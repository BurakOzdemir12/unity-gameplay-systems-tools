using _Project.Systems.HealthSystem.Structs;

namespace _Project.Systems.HealthSystem.Health.Interfaces
{
    public interface IDamageable
    {
        // void ApplyDamage(float damage);
        void ApplyDamage(DamageInfo damageInfo);
    }
}