using _Project.Systems.InventorySystem;
using _Project.Systems.InventorySystem.ScriptableObjects;
using _Project.Systems.SharedGameplay.Pickup_Drop.Interfaces;

namespace _Project.Systems.SharedGameplay.WeaponLogic
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