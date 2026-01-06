using UnityEngine;

namespace _Project.Systems._Core.GravityForce.Interfaces
{
    public interface IKnockable
    {
        void ApplyKnockback(float force,Vector3 direction);
    }
}