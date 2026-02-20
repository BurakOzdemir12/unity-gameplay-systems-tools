using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using _Project.Systems.SharedGameplay.WeaponLogic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Events
{
    public struct WeaponImpactActionEvent : IEvent
    {
        public GameObject Source;
        public GameObject SourceTool;
        public GameObject Target;
        public string Tag;
        public WeaponData WeaponData;
        public SurfaceType Surface;
        public Vector3 Position;
        public Vector3 Normal;

        public WeaponImpactActionEvent(GameObject source, GameObject sourceTool, GameObject target, string tag,
            WeaponData weaponData,
            SurfaceType surface, Vector3 position, Vector3 normal)
        {
            Source = source;
            SourceTool = sourceTool;
            Target = target;
            Tag = tag;
            WeaponData = weaponData;
            Surface = surface;
            Position = position;
            Normal = normal;
        }
    }
}