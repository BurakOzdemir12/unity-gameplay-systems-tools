using System.Collections.Generic;
using _Project.Systems._Core.Components;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.GravityForce.Interfaces;
using _Project.Systems._Core.Health.Interfaces;
using _Project.Systems._Core.WeaponLogic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.WeaponLogic
{
    public class WeaponLogic : MonoBehaviour
    {
        [Header("Weapon Data")] [SerializeField]
        private WeaponDataSo weaponData;

        public WeaponDataSo WeaponData => weaponData;

        [Header("Collider References (Set in Inspector")] [SerializeField]
        private Collider characterOwnCollider;

        private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();

        [Header("Impact Normal Improve (Optional)")] [SerializeField]
        private bool useNormalRaycast = true;

        [SerializeField] private float normalRayDistance = 0.25f;
        [SerializeField] private LayerMask normalRayMask = ~0;

        private bool active;
        private float currentDamage;
        private float currentKnockbackForce;

        private string currentAttackType;

        private void OnEnable() => hitColliders.Clear();

        private void OnDisable()
        {
            hitColliders.Clear();
            active = false;
        }

        public void Initialize(Collider ownerCollider) => characterOwnCollider = ownerCollider;

        public void BeginAttack(float finalDamage, float finalKnockbackForce,
            string impactTag)
        {
            currentDamage = finalDamage;
            currentKnockbackForce = finalKnockbackForce;

            currentAttackType = impactTag;

            hitColliders.Clear();
            active = true;

            gameObject.SetActive(true);
        }

        public void EndAttack()
        {
            active = false;
            hitColliders.Clear();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!active) return;

            if (other == characterOwnCollider) return;
            if (!hitColliders.Add(other)) return;


            ApplyDamageAndKnockback(other);
            PublishImpactEvent(other);
        }

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

        private void PublishImpactEvent(Collider other)
        {
            Vector3 weaponPos = transform.position;
            Vector3 hitPoint = other.ClosestPoint(weaponPos);

            Vector3 hitDir = (hitPoint - weaponPos);
            Vector3 normal = hitDir.sqrMagnitude > 0.0001f ? (-hitDir.normalized) : (-transform.forward);

            if (useNormalRaycast)
            {
                Vector3 rayDir = (hitPoint - weaponPos);
                if (rayDir.sqrMagnitude > 0.0001f)
                {
                    rayDir.Normalize();

                    if (Physics.Raycast(weaponPos, rayDir, out var hit, normalRayDistance, normalRayMask,
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

            var evt = new WeaponImpactActionEvent(
                sourceCharacter,
                gameObject,
                target,
                tag: currentAttackType,
                weaponData: weaponData,
                surface,
                hitPoint,
                normal
            );

            EventBus<WeaponImpactActionEvent>.Publish(evt);
        }
    }
}