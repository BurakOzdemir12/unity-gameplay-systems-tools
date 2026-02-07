using _Project.Systems._Core.Enums;
using _Project.Systems.SharedGameplay.Feedback.Tools_Weapons;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.ToolLogic.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ToolData", menuName = "Scriptable Objects/Tools/Tool Data")]
    public class ToolDataSo : ScriptableObject
    {
        public ToolImpactFeedbackProfile toolImpactFeedbackProfile;
        public ToolType toolType;

        public GameObject trailPrefab;
        public GameObject toolPrefab;
        public float staminaCost;
        public float durability;
    }
}