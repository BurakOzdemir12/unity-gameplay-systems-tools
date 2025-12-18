using _Project.Systems.Core.GravityForce;
using UnityEngine;

namespace _Project.Systems.Core.WeaponLogic.Interfaces
{
    public interface IWeaponOwner
    {
        CharacterController Controller { get; }
        ForceReceiver ForceReceiver { get; }
        Transform Transform { get; }
    }
}