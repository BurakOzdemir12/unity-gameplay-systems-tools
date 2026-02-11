using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Events
{
    public struct CharacterCombatActionEvent : IEvent
    {
        public GameObject Source;
        public SurfaceType Surface;
        public CombatActionType Type;
        public WeaponType WeaponType;
        public string ActionTag;
        public Vector3 Position;

        public CharacterCombatActionEvent(GameObject source, CombatActionType type, WeaponType weaponType,
            SurfaceType surface, Vector3 position,
            string actionTag = "")
        {
            Source = source;
            Type = type;
            WeaponType = weaponType;
            Surface = surface;
            Position = position;
            ActionTag = actionTag;
        }
    }
}