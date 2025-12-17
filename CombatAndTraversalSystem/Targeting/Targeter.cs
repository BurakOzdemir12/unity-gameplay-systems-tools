using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Targeting
{
    public class Targeter : MonoBehaviour
    {
        public List<Target> targets = new List<Target>();

        public Target SelectedTarget { get; private set; }
        [SerializeField] private CinemachineTargetGroup cmTargetGroup;
        [SerializeField] private float targetWeight = 1f;
        [SerializeField] private float targetRadius = 1f;
        [SerializeField] private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Target>(out Target target)) return;
            if (!targets.Contains(target))
            {
                targets.Add(target);
                target.OnTargetDestroyed += RemoveTarget;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<Target>(out Target target)) return;
            if (targets.Contains(target))
            {
                RemoveTarget(target);
            }
        }

        public bool SelectTarget()
        {
            if (targets.Count == 0) return false;

            Target closestTarget = null;
            float closestTargetDistance = Mathf.Infinity;
            foreach (var target in targets)
            {
                Vector2 viewPos = mainCamera.WorldToViewportPoint(target.transform.position);

                if (viewPos is not { x: > 0 and < 1, y: > 0 and < 1 })
                {
                    continue;
                }

                Vector2 targetToCenter = viewPos - new Vector2(0.5f, 0.5f); // center of screen thats why 0,5f,0,5f
                if (targetToCenter.sqrMagnitude < closestTargetDistance)
                {
                    closestTarget = target;
                    closestTargetDistance = targetToCenter.sqrMagnitude;
                }
            }

            if (closestTarget == null)
            {
                return false;
            }

            SelectedTarget = closestTarget;
            if (cmTargetGroup == null)
            {
                Debug.LogError("Targeter has no CineMachine Target Group! You need to assing it");
                return false;
            }

            cmTargetGroup.AddMember(SelectedTarget?.transform, targetWeight, targetRadius);

            return true;
        }

        public void DeselectTarget()
        {
            if (SelectedTarget == null) return;

            if (cmTargetGroup == null)
            {
                Debug.LogError("Targeter has no CineMachine Target Group! You need to assing it");
                return;
            }

            cmTargetGroup.RemoveMember(SelectedTarget?.transform);
            SelectedTarget = null;
        }

        private void RemoveTarget(Target target)
        {
            if (!targets.Contains(target)) return;
            if (SelectedTarget == target)
            {
                cmTargetGroup.RemoveMember(SelectedTarget?.transform);
                SelectedTarget = null;
            }

            target.OnTargetDestroyed -= RemoveTarget;
            targets.Remove(target);
        }
    }
}