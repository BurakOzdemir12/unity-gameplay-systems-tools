using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.EventBus.Events
{
    public struct CharacterTraversalEvent : IEvent
    {
        public GameObject Source;
        public TraversalType Type;
        public SurfaceType Surface;
        public string ActionTag;
        public Vector3 Position;

        public CharacterTraversalEvent(GameObject source, TraversalType type, SurfaceType surface, Vector3 position, string actionTag = "")
        {
            Source = source;
            Type = type;
            Surface = surface;
            Position = position;
            ActionTag = actionTag;
        }
    }
}
