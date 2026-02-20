using _Project.Systems.InventorySystem;
using _Project.Systems.InventorySystem.Core;
using _Project.Systems.InventorySystem.ScriptableObjects;
using _Project.Systems.InventorySystem.UI;
using _Project.Systems.SharedGameplay.Pickup_Drop.Interfaces;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.Pickup_Drop
{
    public class PickupController : MonoBehaviour
    {
        [field: SerializeField] public InventoryComponent InventoryComponent { get; private set; }

        // public event Action OnPickup;
        [SerializeField] private float pickUpRange;
        [SerializeField] private LayerMask pickupableLayer;
        [SerializeField] private Vector3 dropOffset;

        private void OnEnable()
        {
            InventoryComponent.OnItemDroppedToWorld += HandleItemDropped;
        }


        public void TryPickup()
        {
            if (InventoryComponent == null)
            {
                InventoryComponent = GetComponentInParent<InventoryComponent>();
                if (InventoryComponent == null)
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

            InventoryComponent.AddItem(pickable.Data, pickable.Amount);
            pickable.OnPickedUp();
        }

        private void HandleItemDropped(ItemData data, int amount)
        {
            var dropPoint = transform.TransformPoint(dropOffset);
            GameObject droppedItem = Instantiate(data.itemPrefab, dropPoint, Quaternion.identity);
            
            if (droppedItem.TryGetComponent<Item>(out var item))
            {
                item.CurrentItemData = data;
                item.CurrentItemAmount = amount;
            }
        }

        private void OnDisable()
        {
            InventoryComponent.OnItemDroppedToWorld -= HandleItemDropped;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.darkMagenta;
            Gizmos.DrawWireSphere(transform.position, pickUpRange);
        }
    }
}