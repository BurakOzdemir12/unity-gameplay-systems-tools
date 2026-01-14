using System;
using UnityEngine;

namespace _Project.Systems.GatheringSystem.Detector_Controller
{
    public class GatheringController : MonoBehaviour
    {
        [field: SerializeField] public GatheringValidator GatheringValidator { get; private set; }
        public GatherTargetData CurrentResourcesData { get; private set; }

        public bool HasValidResource => CurrentResourcesData.HasTarget;

        private void Update()
        {
        }

        public bool TryGatherAction()
        {
            if (!GatheringValidator) return false;

            CurrentResourcesData = GatheringValidator.CheckForGatherableResources();

            // Debug.Log(CurrentResourcesData.TargetTransform.name);
            return CurrentResourcesData.HasTarget;
        }

        public void CancelGatherAction()
        {
            // loop stop / reset target
            // stop hit window
            // UI cancel
        }
    }
}