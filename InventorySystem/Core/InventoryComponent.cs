using System;
using System.Collections.Generic;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _Project.Systems.InventorySystem.Core
{
    public class InventoryComponent : MonoBehaviour
    {
        [Header("Test Items")] [SerializeField]
        private ItemData swordItem;

        [SerializeField] private ItemData pickaxeItem;

        [Header("Inventory Settings")] public int maxSlots = 40;

        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
        public IReadOnlyList<InventorySlot> Slots => slots;

        public event Action<int, InventorySlot> OnSlotChanged;
        public event Action<ItemData, int> OnItemDroppedToWorld;
        public event Action OnInventoryToggle;

        private void Awake()
        {
            for (int i = 0; i < maxSlots; i++)
            {
                slots.Add(new InventorySlot());
            }
        }

        private void Update()
        {
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                AddItem(swordItem, 1);
            }
            else if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                AddItem(pickaxeItem, 1);
            }
        }

        public void SwapOrMergeSlots(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex) return;
            InventorySlot fromSlot = slots[fromIndex];
            InventorySlot toSlot = slots[toIndex];

            if (!fromSlot.HasItem) return;

            //? If is same item and stackable, stack them
            if (toSlot.HasItem && toSlot.StoredItem == fromSlot.StoredItem && toSlot.StoredItem.isStackable)
            {
                int spaceLeft = toSlot.StoredItem.maxStackSize - toSlot.ItemAmount;

                if (spaceLeft > 0)
                {
                    int amountToMove = Mathf.Min(spaceLeft, fromSlot.ItemAmount);

                    toSlot.AddAmount(amountToMove);
                    fromSlot.RemoveAmount(amountToMove);

                    OnSlotChanged?.Invoke(fromIndex, fromSlot);
                    OnSlotChanged?.Invoke(toIndex, toSlot);
                    return;
                }
            }

            //? If is different items or toSlot is empty, swap them
            ItemData tempItem = toSlot.StoredItem;
            int tempAmount = toSlot.ItemAmount;

            toSlot.SetItem(fromSlot.StoredItem, fromSlot.ItemAmount);

            if (tempAmount > 0)
                fromSlot.SetItem(tempItem, tempAmount);
            else
                fromSlot.ClearSlot();

            OnSlotChanged?.Invoke(fromIndex, fromSlot);
            OnSlotChanged?.Invoke(toIndex, toSlot);
        }

        public void AddItem(ItemData itemToAdd, int amount)
        {
            int remaining = amount;

            if (itemToAdd.isStackable)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].HasItem && slots[i].StoredItem == itemToAdd)
                    {
                        int currentAmount = slots[i].ItemAmount;
                        int maxStack = itemToAdd.maxStackSize;

                        if (currentAmount < maxStack)
                        {
                            int spaceLeft = maxStack - currentAmount;
                            int amountToAdd = Mathf.Min(spaceLeft, remaining);

                            slots[i].AddAmount(amountToAdd);
                            remaining -= amountToAdd;

                            OnSlotChanged?.Invoke(i, slots[i]);
                            if (remaining <= 0) return;
                        }
                    }
                }
            }

            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].HasItem)
                {
                    int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);

                    slots[i].SetItem(itemToAdd, amountToPlace);
                    remaining -= amountToPlace;

                    OnSlotChanged?.Invoke(i, slots[i]);

                    if (remaining <= 0) return;
                }
            }

            if (remaining > 0)
            {
                Debug.LogError("Not enough space in inventory to add item: " + itemToAdd.name);
            }
        }

        public void DropItemFromSlot(int index)
        {
            InventorySlot slot = slots[index];
            if (!slot.HasItem) return;

            OnItemDroppedToWorld?.Invoke(slot.StoredItem, slot.ItemAmount);

            slot.ClearSlot();
            OnSlotChanged?.Invoke(index, slot);
        }

        public void ToggleInventoryVisibility()
        {
            OnInventoryToggle?.Invoke();
        }
    }
}