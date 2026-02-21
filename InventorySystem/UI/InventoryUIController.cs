using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Systems.InventorySystem.Core;
using _Project.Systems.InventorySystem.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
        [SerializeField] private Image dragIcon;
        [SerializeField] private GameObject dropArea;

        [Header("Inventory Weight Settings")] [Tooltip("Weight Slider")] [SerializeField]
        private Slider weightSlider;

        [Tooltip("Current weight Text")] [SerializeField]
        private TextMeshProUGUI currentWeightText;

        [Tooltip("Max weight Text")] [SerializeField]
        private TextMeshProUGUI maxWeightText;

        [Tooltip("Weight limit reached warning text")] [SerializeField]
        private TextMeshProUGUI weightLimitWarningText;

        [Tooltip("Weight limit icon")] [SerializeField]
        private RawImage weightIcon;

        [Tooltip("Weight limit message shown time")] [SerializeField]
        private float weightLimitMessageTime = 3f;


        //Slot Settings
        private List<SlotUI> inventorySlots = new List<SlotUI>();
        private SlotUI draggedSlotUi = null;
        private SlotUI hovered;
        private UnityEngine.UI.ScrollRect currentScrollRect;

        //coroutines
        private Coroutine inventoryMessageCoroutine;

        private void Awake()
        {
            inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<SlotUI>(true));

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].Initialize(i);
            }

            currentWeightText.text = inventoryComponent.CurrentWeight.ToString("0.##");
            maxWeightText.text = $"/ {inventoryComponent.MaxWeight:0.##}";
            weightSlider.value = inventoryComponent.CurrentWeight / inventoryComponent.MaxWeight;
        }

        private void OnEnable()
        {
            if (inventoryComponent == null) return;

            inventoryComponent.OnSlotChanged += UpdateSlotUI;
            inventoryComponent.OnInventoryToggle += ToggleInventoryVisibility;
            inventoryComponent.OnWeightChanged += UpdateWeight;
            inventoryComponent.OnEncumbered += HandleEncumbered;
        }

        private void OnDisable()
        {
            if (inventoryComponent == null) return;

            inventoryComponent.OnSlotChanged -= UpdateSlotUI;
            inventoryComponent.OnInventoryToggle -= ToggleInventoryVisibility;
            inventoryComponent.OnWeightChanged -= UpdateWeight;
            inventoryComponent.OnEncumbered -= HandleEncumbered;
        }

        private void HandleEncumbered(string message)
        {
            weightLimitWarningText.text = message;
            weightLimitWarningText.gameObject.SetActive(true);

            if (inventoryMessageCoroutine != null) StopCoroutine(EncumberedMessageRoutine());
            inventoryMessageCoroutine = StartCoroutine(EncumberedMessageRoutine());
        }

        private IEnumerator EncumberedMessageRoutine()
        {
            yield return new WaitForSeconds(weightLimitMessageTime);

            weightLimitWarningText.gameObject.SetActive(false);
        }

        private void UpdateWeight(bool isWeightLimitReached)
        {
            currentWeightText.text = inventoryComponent.CurrentWeight.ToString("0.##");
            maxWeightText.text = $"/{inventoryComponent.MaxWeight:0.##}";
            weightSlider.value = inventoryComponent.CurrentWeight / inventoryComponent.MaxWeight;

            weightIcon.color = isWeightLimitReached ? Color.red : Color.gray6;
        }

        private void ToggleInventoryVisibility()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
            Cursor.lockState = inventoryPanel.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = inventoryPanel.activeInHierarchy;
        }

        private void UpdateSlotUI(int index, InventorySlot updatedSlot)
        {
            if (index >= 0 && index < inventorySlots.Count)
            {
                inventorySlots[index].UpdateUi(updatedSlot);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
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