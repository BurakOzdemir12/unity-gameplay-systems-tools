using System;
using _Project.Systems.InventorySystem.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Systems.InventorySystem
{
    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool hovering;
        public bool Hovering => hovering;

        private ItemDataSo storedItem;
        public ItemDataSo StoredItem => storedItem;

        private int itemAmount;
        public int ItemAmount => itemAmount;

        private Image iconImage;
        private TextMeshProUGUI amountText;

        [SerializeField] private GameObject selectedHotbarHighlight;

        private void Awake()
        {
            iconImage = transform.GetChild(0).GetComponent<Image>();
            amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        public void SetItem(ItemDataSo item, int amount = 1)
        {
            storedItem = item;
            itemAmount = amount;
            RefreshSlot();
        }

        public void RefreshSlot()
        {
            if (iconImage == null)
            {
                iconImage = transform.GetChild(0).GetComponent<Image>();
                amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            }

            if (storedItem != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = storedItem.itemIcon;
                amountText.text = itemAmount.ToString();
            }
            else
            {
                iconImage.enabled = false;
                amountText.text = "";
            }
        }

        public int AddAmount(int amount)
        {
            itemAmount += amount;
            RefreshSlot();
            return itemAmount;
        }

        public int RemoveAmount(int amount)
        {
            itemAmount -= amount;
            if (itemAmount <= 0)
            {
                ClearSlot();
            }
            else
            {
                RefreshSlot();
            }

            return itemAmount;
        }

        public void ClearSlot()
        {
            storedItem = null;
            itemAmount = 0;
            RefreshSlot();
        }

        public bool HasItem => storedItem != null;

        public void SetSelected(bool selected)
        {
            if (selectedHotbarHighlight != null)
                selectedHotbarHighlight.SetActive(selected);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovering = false;
        }
    }
}