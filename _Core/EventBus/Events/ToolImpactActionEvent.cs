using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus.Interfaces;
using _Project.Systems._Core.ToolLogic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.EventBus.Events
{
    public struct ToolImpactActionEvent : IEvent
    {
        public GameObject Source;
        public GameObject SourceTool;
        public GameObject Target;
        public string Tag;
        public ToolDataSo ToolData;
        public SurfaceType Surface;
        public Vector3 Position;
        public Vector3 Normal;

        public ToolImpactActionEvent(GameObject source, GameObject sourceTool, GameObject target, string tag,
            ToolDataSo toolData, SurfaceType surface, Vector3 position, Vector3 normal)
        {
            Source = source;
            SourceTool = sourceTool;
            Target = target;
            Tag = tag;
            ToolData = toolData;
            Surface = surface;
            Position = position;
            Normal = normal;
        }
    }
}