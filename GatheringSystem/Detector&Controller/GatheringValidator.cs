using System;
using _Project.Systems.GatheringSystem.Interfaces;
using Unity.XR.OpenVR;
using UnityEngine;

namespace _Project.Systems.GatheringSystem.Detector_Controller
{
    public class GatheringValidator : MonoBehaviour
    {
        [Header("Non Alloc Spherecast Settings")] [SerializeField]
        private LayerMask resourcesLayerMask;

        [SerializeField] private float rayDistance;
        [SerializeField] private float rayRadius;
        [SerializeField] private Vector3 rayOffset;
        [SerializeField] private int maxBuff;
        private Collider[] hitColliders;

        private GatherTargetData foundResourcesData;
        public GatherTargetData FoundResourcesData => foundResourcesData;

        [Space(10)] [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            hitColliders = new Collider[maxBuff];
            if (!mainCamera) mainCamera = Camera.main;
        }

        public GatherTargetData CheckForGatherableResources()
        {
            var hitData = new GatherTargetData
            {
                HasTarget = false,
                Target = null,
                TargetTransform = null,
                InteractPoint = default
            };

            Vector3 origin = transform.position + rayOffset;
            int hitCount = Physics.OverlapSphereNonAlloc(origin, rayRadius,
                hitColliders, resourcesLayerMask);

            IGatherable closestGatherable = null;
            Transform closestGatherableTransform = null;
            float closestGatherableDistance = Mathf.Infinity;
            Vector3 closestPoint = default;

            for (int i = 0; i < hitCount; i++)
            {
                var col = hitColliders[i];

                if (!col) continue;
                if (!col.TryGetComponent(out IGatherable gatherable)) continue;

                var colTransform =
                    gatherable.InteractTransform != null
                        ? gatherable.InteractTransform
                        : col.transform;

                Vector3 worldPos = colTransform.position;
                Vector2 viewPos = mainCamera.WorldToViewportPoint(worldPos);
                // if (viewPos.z <= 0f) continue;
                if (viewPos.x <= 0 || viewPos.x >= 1 || viewPos.y <= 0 || viewPos.y >= 1)
                    continue;

                Vector2 toCenter = viewPos - new Vector2(0.5f, 0.5f);
                if (toCenter.sqrMagnitude < closestGatherableDistance)
                {
                    closestGatherable = gatherable;
                    closestGatherableDistance = toCenter.sqrMagnitude;

                    closestGatherableTransform = colTransform;
                    closestPoint = worldPos;
                }
            }


            if (closestGatherable == null)
            {
                foundResourcesData = hitData;
                return hitData;
            }

            hitData.HasTarget = true;
            hitData.Target = closestGatherable;
            hitData.TargetTransform = closestGatherableTransform;
            hitData.InteractPoint = closestPoint;

            foundResourcesData = hitData;

            return hitData;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = foundResourcesData.HasTarget ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + rayOffset, rayRadius);
        }
    }
}

public struct GatherTargetData
{
    public bool HasTarget;
    public IGatherable Target;
    public Transform TargetTransform;
    public Vector3 InteractPoint;
}