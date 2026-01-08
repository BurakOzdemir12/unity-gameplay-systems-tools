using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.EventBus.Events
{
    public struct CharacterInteractionEvent : IEvent
    {
        public GameObject Source;
        public InteractionType Type;
        public SurfaceType Surface;
        public string ActionTag;
        public Vector3 Position;

        public CharacterInteractionEvent(GameObject source, InteractionType type, SurfaceType surface, Vector3 position, string actionTag = "")
        {
            Source = source;
            Type = type;
            Surface = surface;
            Position = position;
            ActionTag = actionTag;
        }
    }
}
