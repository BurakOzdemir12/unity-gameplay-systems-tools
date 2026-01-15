using UnityEngine;

namespace _Project.Systems.GatheringSystem.Interfaces
{
    public interface IGatherable
    {
        Transform InteractTransform { get; }
        // GameObject RequiredTool { get; }
    }
}