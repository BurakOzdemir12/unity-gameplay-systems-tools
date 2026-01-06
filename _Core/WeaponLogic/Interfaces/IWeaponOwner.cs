using _Project.Systems._Core.GravityForce;
using UnityEngine;

namespace _Project.Systems._Core.WeaponLogic.Interfaces
{
    public interface IWeaponOwner
    {
        CharacterController Controller { get; }
        ForceReceiver ForceReceiver { get; }
        Transform Transform { get; }
    }
}