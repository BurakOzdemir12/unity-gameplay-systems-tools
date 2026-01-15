using _Project.Systems._Core.Enums;
using _Project.Systems._Core.Feedback.Tools_Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems._Core.ToolLogic.ScriptableObjects
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