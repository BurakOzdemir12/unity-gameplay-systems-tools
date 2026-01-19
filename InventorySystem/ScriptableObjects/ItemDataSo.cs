using UnityEngine;

namespace _Project.Systems.InventorySystem.ScriptableObjects
{
    public class ItemDataSo : ScriptableObject
    {
        public int itemID;
        public string itemName;
        public Sprite itemIcon;
        public int maxStackSize;
        public bool isStackable;
        public GameObject itemPrefab;
        public GameObject handItemPrefab;
    }
}