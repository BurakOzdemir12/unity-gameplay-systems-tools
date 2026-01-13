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
            if (!GatheringValidator) return;
            CurrentResourcesData = GatheringValidator.CheckForGatherableResources();
            if (CurrentResourcesData.HasTarget)
                Debug.Log(CurrentResourcesData.TargetTransform.name);
        }

        public void TryGatherAction()
        {
            
        }
    }
}