using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.InventorySystem.Core
{
    [System.Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemData storedItem;
        public ItemData StoredItem => storedItem;

        [SerializeField] private int itemAmount;
        public int ItemAmount => itemAmount;

        public bool HasItem => storedItem != null && itemAmount > 0;

        public InventorySlot(ItemData itemData, int amount = 0)
        {
            storedItem = itemData;
            itemAmount = amount;
        }

        public InventorySlot()
        {
            ClearSlot();
        }

        public void SetItem(ItemData itemData, int amount)
        {
            storedItem = itemData;
            itemAmount = amount;
        }

        public void AddAmount(int amount)
        {
            itemAmount += amount;
            //Refresh
        }

        public void RemoveAmount(int amount)
        {
            itemAmount -= amount;
            if (itemAmount <= 0) ClearSlot();
        }

        public void ClearSlot()
        {
            storedItem = null;
            itemAmount = -1;
        }
    }
}