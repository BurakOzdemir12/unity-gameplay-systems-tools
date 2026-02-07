using System.Collections.Generic;
using _Project.Systems._Core.Components;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.GravityForce.Interfaces;
using _Project.Systems.HealthSystem.Health.Interfaces;
using _Project.Systems.SharedGameplay.ToolLogic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.ToolLogic
{
    public class ToolLogic : MonoBehaviour
    {
        [Header("Tool Data")] [SerializeField] private ToolDataSo toolData;
        public ToolDataSo ToolData => toolData;

        [Header("Collider References (Set in Inspector")] [SerializeField]
        private Collider characterOwnCollider;

        private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();

        [Header("Impact Normal Improve (Optional)")] [SerializeField]
        private bool useNormalRaycast = true;

        [SerializeField] private float normalRayDistance = 0.25f;
        [SerializeField] private LayerMask normalRayMask = ~0;

        private bool hitWindowActive;
        private float currentDamage;
        private float currentKnockbackForce;

        private string currentLootActionType;
        private Vector3 impactPointDebug;

        private void OnEnable() => hitColliders.Clear();

        private void OnDisable()
        {
            hitColliders.Clear();
            hitWindowActive = false;
        }

        public void Initialize(Collider ownerCollider) => characterOwnCollider = ownerCollider;


        public void SetupLootAction(float finalDamage, float finalKnockbackForce, string lootActionType)
        {
            currentDamage = finalDamage;
            currentLootActionType = lootActionType;
            currentKnockbackForce = finalKnockbackForce;
        }

        public void PerformLootAction()
        {
            hitColliders.Clear();
            hitWindowActive = true;

            gameObject.SetActive(true);
        }

        public void EndLootAction()
        {
            hitWindowActive = false;
            hitColliders.Clear();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hitWindowActive) return;

            if (other == characterOwnCollider) return;
            if (!hitColliders.Add(other)) return;

            // Debug.Log("Hit collider " + other.name + ": Tag:" + other.tag);

            ApplyDamageAndKnockback(other);
            PublishImpactEvent(other);
        }

        #region Apply Damage And Knockback

        private void ApplyDamageAndKnockback(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable) && damageable != null)
            {
                damageable.ApplyDamage(currentDamage);
            }

            if (other.TryGetComponent<IKnockable>(out var knockable) && knockable != null)
            {
                Vector3 dir = (other.transform.position - transform.root.position);
                dir.y = 0f;
                knockable.ApplyKnockback(currentKnockbackForce, dir);
            }
        }

        #endregion

        #region Impact Events

        private void PublishImpactEvent(Collider other)
        {
            Vector3 toolPos = transform.position;
            Vector3 hitPoint = other.ClosestPoint(toolPos);

            Vector3 hitDir = (hitPoint - toolPos);
            Vector3 normal = hitDir.sqrMagnitude > 0.0001f ? (-hitDir.normalized) : (-transform.forward);

            if (useNormalRaycast)
            {
                Vector3 rayDir = (hitPoint - toolPos);
                if (rayDir.sqrMagnitude > 0.0001f)
                {
                    rayDir.Normalize();

                    if (Physics.Raycast(toolPos, rayDir, out var hit, normalRayDistance, normalRayMask,
                            QueryTriggerInteraction.Ignore))
                    {
                        normal = hit.normal;
                        hitPoint = hit.point;
                    }
                }
            }

            SurfaceType surface = SurfaceType.Dirt; // fallback
            var def = other.GetComponentInParent<SurfaceDefinition>();
            if (def != null)
            {
                surface = def.SurfaceType;
            }

            GameObject sourceCharacter = transform.root.gameObject;
            GameObject target = other.gameObject;

            var evt = new ToolImpactActionEvent(
                sourceCharacter,
                gameObject,
                target,
                tag: currentLootActionType,
                toolData,
                surface,
                hitPoint,
                normal
            );
            impactPointDebug = hitPoint;
            EventBus<ToolImpactActionEvent>.Publish(evt);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = hitColliders.Count > 0 ? Color.paleGreen : Color.red;
            Gizmos.DrawWireCube(impactPointDebug, Vector3.one * 0.2f);
        }
    }
}