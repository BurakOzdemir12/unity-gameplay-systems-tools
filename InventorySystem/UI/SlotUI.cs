using _Project.Systems.InventorySystem.Core;
using _Project.Systems.InventorySystem.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Systems.InventorySystem.UI
{
    public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject selectedHotbarHighlight;
        private bool hovering;
        public bool Hovering => hovering;
        public int SlotIndex { get; private set; }

        private Image iconImage;
        private TextMeshProUGUI amountText;


        private void EnsureReferences()
        {
            if (iconImage == null) iconImage = transform.GetChild(0).GetComponent<Image>();
            if (amountText == null) amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        public void Initialize(int index)
        {
            SlotIndex = index;
            EnsureReferences();
        }

        public void UpdateUi(InventorySlot slotData)
        {
            if (slotData != null && slotData.HasItem)
            {
                iconImage.enabled = true;
                iconImage.sprite = slotData.StoredItem.icon;

                amountText.text = slotData.ItemAmount > 1 ? slotData.ItemAmount.ToString() : "";
            }
            else
            {
                iconImage.enabled = false;
                iconImage.sprite = null;
                amountText.text = "";
            }
        }

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