using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.EventBus.Events
{
    public struct CharacterImpactActionEvent : IEvent
    {
        public GameObject Source;
        public GameObject Target;
        public ImpactActionType ActionType;
        public SurfaceType Surface;
        public Vector3 Normal;

        public CharacterImpactActionEvent(GameObject source, GameObject target, ImpactActionType actionType,
            SurfaceType surface, Vector3 normal)
        {
            Source = source;
            Target = target;
            ActionType = actionType;
            Surface = surface;
            Normal = normal;
        }
    }
}