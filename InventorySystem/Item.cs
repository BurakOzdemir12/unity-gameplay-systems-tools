using _Project.Systems.InventorySystem.Enums;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems.InventorySystem
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private int currentItemAmount;
        [SerializeField] private ItemType itemType;
        public ItemType ItemType => itemType;

        public int CurrentItemAmount
        {
            get => currentItemAmount;
            set => currentItemAmount = value;
        }

        [FormerlySerializedAs("currentInventoryItemData")] [SerializeField]
        private ItemData currentItemData;

        public ItemData CurrentItemData
        {
            get => currentItemData;
            set => currentItemData = value;
        }
    }
}