using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.InventorySystem
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private int currentItemAmount;

        public int CurrentItemAmount
        {
            get => currentItemAmount;
            set => currentItemAmount = value;
        }

        [SerializeField] private ItemDataSo currentItemData;

        public ItemDataSo CurrentItemData
        {
            get => currentItemData;
            set => currentItemData = value;
        }
    }
}