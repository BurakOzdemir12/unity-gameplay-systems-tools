using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.EventBus.Events
{
    public struct CharacterCombatActionEvent : IEvent
    {
        public GameObject Source;
        public SurfaceType Surface;
        public CombatActionType Type;
        public WeaponToolType WeaponToolType;
        public string ActionTag;
        public Vector3 Position;

        public CharacterCombatActionEvent(GameObject source, CombatActionType type, WeaponToolType weaponToolType,
            SurfaceType surface, Vector3 position,
            string actionTag = "")
        {
            Source = source;
            Type = type;
            WeaponToolType = weaponToolType;
            Surface = surface;
            Position = position;
            ActionTag = actionTag;
        }
    }
}