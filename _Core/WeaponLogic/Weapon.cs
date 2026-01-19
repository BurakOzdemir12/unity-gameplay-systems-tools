using System;
using _Project.Systems._Core.Pickup_Drop.Interfaces;
using _Project.Systems.InventorySystem;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.WeaponLogic
{
    public class Weapon : Item, IPickupable
    {
        
        public ItemDataSo Data => CurrentItemData;
        public int Amount => CurrentItemAmount;

        public bool OnPickedUp()
        {
            this.gameObject.SetActive(false);
            return true;
        }
    }
}