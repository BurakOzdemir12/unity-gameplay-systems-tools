using System;
using System.Transactions;
using _Project.Systems._Core.Pickup_Drop.Interfaces;
using _Project.Systems._Core.Weapon_Tool_Handlers;
using _Project.Systems._Core.WeaponLogic;
using _Project.Systems._Core.WeaponLogic.ScriptableObjects;
using _Project.Systems.InventorySystem;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.Pickup_Drop
{
    public class PickupController : MonoBehaviour
    {
        [field: SerializeField] public InventoryManager InventoryManager { get; private set; }

        // public event Action OnPickup;
        [SerializeField] private float pickUpRange;
        [SerializeField] private LayerMask pickupableLayer;
        [SerializeField] private Vector3 dropOffset;

        private void OnEnable()
        {
            InventoryManager.ItemDropped += HandleItemDropped;
        }


        public void TryPickup()
        {
            if (InventoryManager == null)
            {
                InventoryManager = GetComponentInParent<InventoryManager>();
                if (InventoryManager == null)
                {
                    Debug.LogError("[PickupController] Inventory is NULL.");
                    return;
                }
            }

            var cam = Camera.main;
            if (cam == null) return;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (!Physics.Raycast(ray, out var hit, pickUpRange,
                    pickupableLayer))
                return;

            var pickable = hit.collider.GetComponentInParent<IPickupable>();
            if (pickable == null) return;

            if (pickable.Data == null)
            {
                Debug.LogError($"[PickupController] Pickable Data is NULL on {hit.collider.name}");
                return;
            }

            InventoryManager.AddItem(pickable.Data, pickable.Amount);
            pickable.OnPickedUp();
        }

        private void HandleItemDropped(GameObject obj)
        {
            var dropPoint =  transform.TransformPoint(dropOffset);
            GameObject droppedItem = Instantiate(obj, dropPoint, Quaternion.identity);
        }

        private void OnDisable()
        {
            InventoryManager.ItemDropped -= HandleItemDropped;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.darkMagenta;
            Gizmos.DrawWireSphere(transform.position, pickUpRange);
        }
    }
}