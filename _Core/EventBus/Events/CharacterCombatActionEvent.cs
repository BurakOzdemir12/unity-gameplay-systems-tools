using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.EventBus.Events
{
    public struct CharacterCombatActionEvent : IEvent
    {
        public GameObject Source;
        public ActionType Type;
        public SurfaceType Surface;
        public string ActionTag;
        public Vector3 Position;

        public CharacterCombatActionEvent(GameObject source, ActionType type, SurfaceType surface, Vector3 position,
            string actionTag = "")
        {
            Source = source;
            Type = type;
            Surface = surface;
            Position = position;
            ActionTag = actionTag;
        }
    }
}