using _Project.Systems._Core.Weapon_Tool_Handlers;
using _Project.Systems.InventorySystem;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.Pickup_Drop.Interfaces
{
    public interface IPickupable
    {
        ItemDataSo Data { get; }
        int Amount { get; }
        bool OnPickedUp();
    }
}