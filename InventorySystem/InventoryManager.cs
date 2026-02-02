using System;
using System.Collections.Generic;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

namespace _Project.Systems.InventorySystem
{
    public class InventoryManager : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler,
        IDragHandler,
        IDropHandler
    {
        [field: SerializeField] public HotbarManager HotbarManager { get; private set; }

        [SerializeField] private GameObject inventoryPanel;

        [SerializeField] private ItemDataSo swordItem;
        [SerializeField] private ItemDataSo pickaxeItem;

        [SerializeField] private GameObject inventorySlotParent;

        [SerializeField] public Image dragIcon;

        [SerializeField] private GameObject dropArea;

        private List<Slot> inventorySlots = new List<Slot>();
        private List<Slot> allSlots = new List<Slot>();

        private Slot draggedSlot = null;
        private Slot hovered;

        public event Action<GameObject> ItemDropped;

        private void Awake()
        {
            inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());

            allSlots.AddRange(inventorySlots);
            allSlots.AddRange(HotbarManager.hotbarSlots);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                AddItem(swordItem, 3);
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                AddItem(pickaxeItem, 1);
            }
        }

        public void ToggleInventoryVisibility()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }


        public void AddItem(ItemDataSo itemToAdd, int amount)
        {
            int remaining = amount;
            foreach (Slot slot in allSlots)
            {
                if (slot.HasItem && slot.StoredItem == itemToAdd)
                {
                    int currentAmount = slot.ItemAmount;
                    int maxStack = itemToAdd.maxStackSize;
                    if (currentAmount < maxStack)
                    {
                        int spaceLeft = maxStack - currentAmount;
                        int amountToAdd = Mathf.Min(spaceLeft, remaining);
                        slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                        remaining -= amountToAdd;

                        if (remaining <= 0) return;
                    }
                }
            }

            foreach (Slot slot in allSlots)
            {
                if (!slot.HasItem)
                {
                    int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                    slot.SetItem(itemToAdd, amountToPlace);
                    remaining -= amountToPlace;
                    if (remaining <= 0)
                    {
                        return;
                    }
                }
            }

            if (remaining > 0)
            {
                Debug.LogError("Not enough space in inventory to add item: " + itemToAdd.name);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer down");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.pointerEnter == null) return;

            hovered = GetSlotFromEvent(eventData);
            if (hovered != null && hovered.HasItem)
            {
                draggedSlot = hovered;

                dragIcon.sprite = hovered.StoredItem.itemIcon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (hovered != null)
            {
                dragIcon.enabled = false;
                draggedSlot = null;
            }
        }

        private void HandleDrop(Slot from, Slot to)
        {
            if (from == to) return;
            //Stacking
            if (to.HasItem && to.StoredItem == from.StoredItem)
            {
                int max = to.StoredItem.maxStackSize;
                int space = max - to.ItemAmount;

                if (space > 0)
                {
                    int move = Mathf.Min(space, from.ItemAmount);

                    to.SetItem(to.StoredItem, to.ItemAmount + move);
                    from.SetItem(from.StoredItem, from.ItemAmount - move);

                    if (from.ItemAmount <= 0)
                        from.ClearSlot();

                    return;
                }
            }

            //When Different Item
            if (to.HasItem)
            {
                ItemDataSo tempItem = to.StoredItem;
                int tempAmount = to.ItemAmount;
                to.SetItem(from.StoredItem, from.ItemAmount);
                from.SetItem(tempItem, tempAmount);
                return;
            }

            //Empty Slot
            if (!from.HasItem) return;

            to.SetItem(from.StoredItem, from.ItemAmount);
            from.ClearSlot();
        }


        private void DropToWorld(Slot slot)
        {
            if (slot == null) return;

            ItemDataSo itemData = slot.StoredItem;
            GameObject prefab = itemData.itemPrefab;
            if (prefab == null) return;

            if (Camera.main != null)
            {
                ItemDropped?.Invoke(prefab);
                // GameObject droppedItem = Instantiate(prefab,
                //     Camera.main.transform.position + Camera.main.transform.forward,
                //     Quaternion.identity);

                // Item item = droppedItem.GetComponent<Item>();
                Item item = prefab.GetComponent<Item>();
                item.CurrentItemData = itemData;
                item.CurrentItemAmount = slot.ItemAmount;
            }

            slot.ClearSlot();
            // AddItem(item, 1);
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragIcon.transform.position = eventData.position;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("InventoryManager OnDrop CALLED");

            if (draggedSlot == null) return;

            var droppedOn = eventData.pointerCurrentRaycast.gameObject;

            if (droppedOn != null)
            {
                bool isOnDropArea = droppedOn == dropArea || droppedOn.transform.IsChildOf(dropArea.transform);
                if (isOnDropArea)
                {
                    Debug.Log("Dropped on Drop Area");
                    DropToWorld(draggedSlot);
                    dragIcon.enabled = false;
                    draggedSlot = null;
                    return;
                }
            }

            var targetSlot = GetSlotFromEvent(eventData);
            if (targetSlot == null) return;

            HandleDrop(draggedSlot, targetSlot);

            dragIcon.enabled = false;
            draggedSlot = null;
        }

        private Slot GetSlotFromEvent(PointerEventData eventData)
        {
            var go = eventData.pointerEnter;
            if (go == null) return null;
            return go.GetComponentInParent<Slot>();
        }
    }
}