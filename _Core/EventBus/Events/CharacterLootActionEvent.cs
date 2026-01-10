using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.EventBus.Events
{
    public struct CharacterLootActionEvent : IEvent
    {
        public GameObject Source;
        public LootActionType Type;
        public SurfaceType Surface;
        public string ActionTag;
        public Vector3 Position;

        public CharacterLootActionEvent(GameObject source, LootActionType type, SurfaceType surface,
            Vector3 position = default, string actionTag = "")
        {
            Type = type;
            Surface = surface;
            Source = source;
            ActionTag = actionTag;
            Position = position;
        }
    }
}