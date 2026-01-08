using _Project.Systems._Core.Components;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using UnityEngine;

namespace _Project.Systems._Core.Feedback
{
    [RequireComponent(typeof(SurfaceDetection))]
    public class CharacterFeedbackBroadcaster : MonoBehaviour
    {
        private SurfaceDetection surfaceDetection;

        private void Awake()
        {
            surfaceDetection = GetComponent<SurfaceDetection>();
        }

        // Called via Animation Event (string format: "Footstep", "Land", "Attack:Claw")
        public void OnAnimEvent(string eventData)
        {
            string[] parts = eventData.Split(':');
            string actionName = parts[0];
            string tag = parts.Length > 1 ? parts[1] : "";

            if (System.Enum.TryParse(actionName, true, out InteractionType type))
            {
                Vector3 pos = transform.position;
                SurfaceType surface = surfaceDetection.GetSurfaceData(pos);

                var evt = new CharacterInteractionEvent(this.gameObject, type, surface, pos, tag);
                EventBus<CharacterInteractionEvent>.Publish(evt);
            }
            else
            {
                Debug.LogWarning($"Unknown InteractionType in AnimEvent: {actionName} on {name}");
            }
        }

        // Direct call helpers for non-animation events (e.g. from code)
        public void BroadcastAction(InteractionType type, string tag = "")
        {
             Vector3 pos = transform.position;
             SurfaceType surface = surfaceDetection.GetSurfaceData(pos);
             var evt = new CharacterInteractionEvent(this.gameObject, type, surface, pos, tag);
             EventBus<CharacterInteractionEvent>.Publish(evt);
        }
    }
}
