using UnityEngine;

namespace _Project.Systems.Core.GravityForce.Interfaces
{
    public interface IKnockable
    {
        void ApplyKnockback(float force,Vector3 direction);
    }
}