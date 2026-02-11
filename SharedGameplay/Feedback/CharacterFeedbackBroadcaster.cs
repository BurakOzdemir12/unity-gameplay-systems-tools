using _Project.Systems._Core.Components;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems.CombatSystem.Events;
using _Project.Systems.MovementSystem.Events;
using _Project.Systems.SharedGameplay.Weapon_Tool_Handlers;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.Feedback
{
    [RequireComponent(typeof(SurfaceDetection))]
    public class CharacterFeedbackBroadcaster : MonoBehaviour
    {
        [SerializeField] private Transform rFoot;
        [SerializeField] private Transform lFoot;

        [SerializeField] private SurfaceDetection surfaceDetection;
        [SerializeField] private WeaponHandler weaponHandler;
        [SerializeField] private ToolHandler toolHandler;

        private void Awake()
        {
            if (!surfaceDetection)
                surfaceDetection = GetComponent<SurfaceDetection>();
            if (!weaponHandler)
                weaponHandler = GetComponent<WeaponHandler>();
            if (!toolHandler)
                toolHandler = GetComponent<ToolHandler>();

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

            var actionName = SplitEventDataIntoComponents(eventData, out var actionTag, out var side, out var spawnPosition);

            if (System.Enum.TryParse(actionName, true, out TraversalType type))
            {
                if (side.Equals("Left", System.StringComparison.OrdinalIgnoreCase))
                    spawnPosition = lFoot.position;
                else if (side.Equals("Right", System.StringComparison.OrdinalIgnoreCase))
                    spawnPosition = rFoot.position;

                SurfaceType surface = surfaceDetection.GetSurfaceData(spawnPosition);

                var evt = new CharacterTraversalEvent(this.gameObject, type, surface, spawnPosition, actionTag);
                EventBus<CharacterTraversalEvent>.Publish(evt);
            }
            else
            {
                Debug.LogWarning($"Unknown InteractionType in AnimEvent: {actionName} on {name}");
            }
        }


        public void OnCombatAnimEvent(string eventData)
        {
            var actionName = SplitEventDataIntoComponents(eventData, out var actionTag, out var side, out var spawnPosition);

            if (System.Enum.TryParse(actionName, true, out CombatActionType type))
            {
                Vector3 pos = transform.position;
                SurfaceType surface = surfaceDetection.GetSurfaceData(pos);
                WeaponType weaponType = weaponHandler.CurrentWeaponLogic.WeaponData.weaponType;
                var evt = new CharacterCombatActionEvent(this.gameObject, type, weaponType, surface, pos, actionTag);
                EventBus<CharacterCombatActionEvent>.Publish(evt);
            }
            else
            {
                Debug.LogWarning($"Unknown Action type in AnimEvent: {actionName} on {name}");
            }
        }

        public void OnGatherAnimEvent(string eventData)
        {
            var actionName = SplitEventDataIntoComponents(eventData, out var actionTag, out var side, out var spawnPosition);

            if (System.Enum.TryParse(actionName, true, out GatherActionType type))
            {
                Vector3 pos = toolHandler.CurrentToolHitBox.transform.position;
                // SurfaceType surface = surfaceDetection.GetSurfaceData(pos);
                ToolType toolType = toolHandler.CurrentToolLogic.ToolData.toolType;

                var evt = new CharacterGatheringActionEvent(this.gameObject, type, toolType, pos, actionTag);
                EventBus<CharacterGatheringActionEvent>.Publish(evt);
            }
            else
            {
                Debug.LogWarning($"Unknown Action type in AnimEvent: {actionName} on {name}");
            }
        }

        private string SplitEventDataIntoComponents(string eventData, out string actionTag, out string side,
            out Vector3 spawnPosition)
        {
            string[] parts = eventData.Split(':');
            string actionName = parts[0];
            actionTag = parts.Length > 1 ? parts[1] : "";
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