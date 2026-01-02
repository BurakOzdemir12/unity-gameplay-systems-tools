using System;
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

        private void Update()
        {
            if (LedgeValidator == null) return;

            CurrentLedgeHitData = LedgeValidator.DetectLedge();
            OnLedgeDataUpdated?.Invoke(CurrentLedgeHitData);

            var hitData = LedgeValidator.DetectLedge();
            if (hitData.IsForwardRaycastHit)
            {
               
                //
                // Debug.Log("Forward Detected Obstacle " + hitData.forwardRayHitInfo.collider.gameObject.name +
                //           "And Object height is " + hitData.heightRayHitInfo.collider.bounds.size.y +
                //           "Object height for a Player " + (hitData.heightRayHitInfo.point.y - transform.position.y)
                // );
            }
        }
    }
}