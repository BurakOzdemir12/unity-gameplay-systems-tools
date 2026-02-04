using System;
using _Project.Systems.ClimbingSystem.Enums;
using _Project.Systems.ClimbingSystem.ObstacleScripts;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.LedgeClimbing
{
    public class ClimbController : MonoBehaviour
    {
        [field: SerializeField] public LedgeValidator LedgeValidator { get; private set; }
        public LedgeHitData CurrentLedgeHitData { get; private set; }
        public bool HasValidLedge => CurrentLedgeHitData.IsValidLedge;

        public event Action<LedgeHitData> OnLedgeDataUpdated;
        private Quaternion TargetRotation { get; set; }

        public ParkourActionType? DetectedActionType { get; private set; }
        public ParkourObstacle DetectedObstacle { get; private set; }
       
        private void Update()
        {
            if (LedgeValidator == null) return;
            CurrentLedgeHitData = LedgeValidator.DetectLedge();
            OnLedgeDataUpdated?.Invoke(CurrentLedgeHitData);
        }

       
    }
}