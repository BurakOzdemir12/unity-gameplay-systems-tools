using _Project.Systems._Core.Weapon_Tool_Handlers;
using UnityEngine;

namespace _Project.Systems._Core.Pickup_Drop.Interfaces
{
    public interface IPickupable
    {
        void PickUp(ScriptableObject context);
    }
}