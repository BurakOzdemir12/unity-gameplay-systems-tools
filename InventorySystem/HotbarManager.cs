using System;
using System.Collections.Generic;
using _Project.Systems._Core;
using _Project.Systems._Core.InputHandler;
using UnityEngine;

namespace _Project.Systems.InventorySystem
{
    public class HotbarManager : MonoBehaviour
    {
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [SerializeField] private GameObject hotbarParent;
        public List<Slot> hotbarSlots = new List<Slot>();

        private int selectedHotbarIndex;

        private void Awake()
        {
            hotbarSlots.AddRange(hotbarParent.GetComponentsInChildren<Slot>());
        }

        private void OnEnable()
        {
            InputHandler.HotbarSelectEvent += HandleHotbarSelection;
            InputHandler.HotbarScrollEvent += HandleScroll;
        }

        private void HandleScroll(int dir)
        {
            selectedHotbarIndex = (selectedHotbarIndex + dir + hotbarSlots.Count) % hotbarSlots.Count;
            ToggleSelectedHighLight();
        }

        private void HandleHotbarSelection(int index)
        {
            if (index < 0 || index >= hotbarSlots.Count) return;

            selectedHotbarIndex = index;
            ToggleSelectedHighLight();
        }

        private void ToggleSelectedHighLight()
        {
            foreach (var slot in hotbarSlots)
                if (slot != hotbarSlots[selectedHotbarIndex])
                    slot.SetSelected(false);

            hotbarSlots[selectedHotbarIndex].SetSelected(true);
        }

        private void OnDisable()
        {
            InputHandler.HotbarSelectEvent -= HandleHotbarSelection;
        }
    }
}