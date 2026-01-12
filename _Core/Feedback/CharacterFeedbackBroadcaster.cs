using _Project.Systems._Core.Components;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems.MovementSystem;
using UnityEngine;

namespace _Project.Systems._Core.Feedback
{
    [RequireComponent(typeof(SurfaceDetection))]
    public class CharacterFeedbackBroadcaster : MonoBehaviour
    {
        [SerializeField] private Transform rFoot;
        [SerializeField] private Transform lFoot;

        private SurfaceDetection surfaceDetection;
        private WeaponHandler weaponHandler;

        private void Awake()
        {
            surfaceDetection = GetComponent<SurfaceDetection>();
            weaponHandler = GetComponent<WeaponHandler>();
            
            if (surfaceDetection == null)
            {
                Debug.LogError($"{name}: Surface Detection not found on character!", this);
            }
            if (weaponHandler == null)
            {
                Debug.LogError($"{name}: WeaponHandler not found on character!", this);
            }
        }

        // Called via Animation Event (string format: "Footstep", "Land", "Attack:Claw")
        public void OnTraversalAnimEvent(string eventData)
        {
            // Debug.Log($"[Broadcaster] OnAnimEvent called: {eventData} at {Time.time}");

            var actionName = SplitEventDataIntoComponents(eventData, out var tag, out var side, out var spawnPosition);

            if (System.Enum.TryParse(actionName, true, out TraversalType type))
            {
                if (side.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                    spawnPosition = lFoot.position;
                else if (side.Equals("Right", System.StringComparison.OrdinalIgnoreCase))
                    spawnPosition = rFoot.position;

                SurfaceType surface = surfaceDetection.GetSurfaceData(spawnPosition);

                var evt = new CharacterTraversalEvent(this.gameObject, type, surface, spawnPosition, tag);
                EventBus<CharacterTraversalEvent>.Publish(evt);
            }
            else
            {
                Debug.LogWarning($"Unknown InteractionType in AnimEvent: {actionName} on {name}");
            }
        }


        public void OnCombatAnimEvent(string eventData)
        {
            var actionName = SplitEventDataIntoComponents(eventData, out var tag, out var side, out var spawnPosition);

            if (System.Enum.TryParse(actionName, true, out CombatActionType type))
            {
                Vector3 pos = transform.position;
                SurfaceType surface = surfaceDetection.GetSurfaceData(pos);
                WeaponToolType weaponToolType = weaponHandler.CurrentWeaponLogic.WeaponData.weaponToolType;
                var evt = new CharacterCombatActionEvent(this.gameObject, type, weaponToolType, surface, pos, tag);
                EventBus<CharacterCombatActionEvent>.Publish(evt);
            }
            else
            {
                Debug.LogWarning($"Unknown Action type in AnimEvent: {actionName} on {name}");
            }
        }

        public void OnLootAnimEvent(string eventData)
        {
            var actionName = SplitEventDataIntoComponents(eventData, out var tag, out var side, out var spawnPosition);

            if (System.Enum.TryParse(actionName, true, out LootActionType type))
            {
                Vector3 pos = transform.position;
                SurfaceType surface = surfaceDetection.GetSurfaceData(pos);
                var evt = new CharacterLootActionEvent(this.gameObject, type, surface, pos, tag);
                EventBus<CharacterLootActionEvent>.Publish(evt);
            }
            else
            {
                Debug.LogWarning($"Unknown Action type in AnimEvent: {actionName} on {name}");
            }
        }

        private string SplitEventDataIntoComponents(string eventData, out string tag, out string side,
            out Vector3 spawnPosition)
        {
            string[] parts = eventData.Split(':');
            string actionName = parts[0];
            tag = parts.Length > 1 ? parts[1] : "";
            side = parts.Length > 2 ? parts[2] : "";

            spawnPosition = transform.position;
            return actionName;
        }

        // Direct call helpers for non-animation events (e.g. from code)
        public void BroadcastAction(TraversalType type, string tag = "")
        {
            Vector3 pos = transform.position;
            SurfaceType surface = surfaceDetection.GetSurfaceData(pos);
            var evt = new CharacterTraversalEvent(this.gameObject, type, surface, pos, tag);
            EventBus<CharacterTraversalEvent>.Publish(evt);
        }
    }
}