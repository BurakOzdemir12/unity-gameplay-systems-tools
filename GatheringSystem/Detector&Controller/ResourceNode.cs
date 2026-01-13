using _Project.Systems.GatheringSystem.Interfaces;
using UnityEngine;

namespace _Project.Systems.GatheringSystem.Detector_Controller
{
    public class ResourceNode : MonoBehaviour, IGatherable
    {
        [SerializeField] private Transform interactPoint;

        public Transform InteractTransform => interactPoint != null
            ? interactPoint
            : transform;
    }
}