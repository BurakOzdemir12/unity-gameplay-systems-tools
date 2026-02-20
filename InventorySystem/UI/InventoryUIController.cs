using System;
using System.Collections.Generic;
using _Project.Systems.InventorySystem.Core;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

namespace _Project.Systems.InventorySystem.UI
{
    public class InventoryUIController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler,
        IDragHandler,
        IDropHandler
    {
        // [field: SerializeField] public HotbarManager HotbarManager { get; private set; }
        [SerializeField] private InventoryComponent inventoryComponent;

        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject inventorySlotParent;
        [SerializeField] public Image dragIcon;
        [SerializeField] private GameObject dropArea;

        private List<SlotUI> inventorySlots = new List<SlotUI>();

        private SlotUI draggedSlotUi = null;
        private SlotUI hovered;

        private void Awake()
        {
            inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<SlotUI>(true));

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].Initialize(i);
            }
        }

        private void OnEnable()
        {
            if (inventoryComponent == null) return;

            inventoryComponent.OnSlotChanged += UpdateSlotUI;
            inventoryComponent.OnInventoryToggle += ToggleInventoryVisibility;
        }

        private void OnDisable()
        {
            if (inventoryComponent == null) return;

            inventoryComponent.OnSlotChanged -= UpdateSlotUI;
            inventoryComponent.OnInventoryToggle -= ToggleInventoryVisibility;
        }

        private void UpdateSlotUI(int index, InventorySlot updatedSlot)
        {
            if (index >= 0 && index < inventorySlots.Count)
            {
                inventorySlots[index].UpdateUi(updatedSlot);
            }
        }

        private void ToggleInventoryVisibility()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
            Cursor.lockState = inventoryPanel.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = inventoryPanel.activeInHierarchy;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer down");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.pointerEnter == null) return;

            hovered = GetSlotFromEvent(eventData);

            if (hovered != null)
            {
                InventorySlot slotData = inventoryComponent.Slots[hovered.SlotIndex];
                if (slotData.HasItem)
                {
                    draggedSlotUi = hovered;
                    dragIcon.sprite = slotData.StoredItem.icon;
                    dragIcon.color = new Color(1, 1, 1, 0.5f);
                    dragIcon.enabled = true;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (hovered != null)
            {
                dragIcon.enabled = false;
                draggedSlotUi = null;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragIcon.enabled)
                dragIcon.transform.position = eventData.position;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("InventoryManager OnDrop CALLED");

            if (draggedSlotUi == null) return;

            var droppedOn = eventData.pointerCurrentRaycast.gameObject;

            if (droppedOn != null)
            {
                bool isOnDropArea = droppedOn == dropArea || droppedOn.transform.IsChildOf(dropArea.transform);
                if (isOnDropArea)
                {
                    inventoryComponent.DropItemFromSlot(draggedSlotUi.SlotIndex);

                    dragIcon.enabled = false;
                    draggedSlotUi = null;
                    return;
                }
            }

            var targetSlotUi = GetSlotFromEvent(eventData);
            if (targetSlotUi == null) return;

            inventoryComponent.SwapOrMergeSlots(draggedSlotUi.SlotIndex, targetSlotUi.SlotIndex);

            dragIcon.enabled = false;
            draggedSlotUi = null;
        }

        private SlotUI GetSlotFromEvent(PointerEventData eventData)
        {
            var go = eventData.pointerEnter;
            if (go == null) return null;
            return go.GetComponentInParent<SlotUI>();
        }
    }
}