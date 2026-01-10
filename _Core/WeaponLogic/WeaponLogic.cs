using System.Collections.Generic;
using _Project.Systems._Core.Components;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.GravityForce.Interfaces;
using _Project.Systems._Core.Health.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.WeaponLogic
{
    public class WeaponLogic : MonoBehaviour
    {
        [SerializeField] private Collider characterOwnCollider;
        private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();
        private SurfaceDetection surfaceDetection;

        private bool active;
        private float currentDamage;
        private float currentKnockbackForce;

        private float currentAttackMultiplier = 1f;

        private void Awake()
        {
            surfaceDetection = GetComponent<SurfaceDetection>();
        }

        private void OnEnable()
        {
            hitColliders.Clear();
        }

        private void OnDisable()
        {
            hitColliders.Clear();
            active = false;
        }

        public void Initialize(Collider ownerCollider)
        {
            characterOwnCollider = ownerCollider;
        }

        public void BeginAttack(float finalDamage, float finalKnockbackForce)
        {
            currentDamage = finalDamage;
            currentKnockbackForce = finalKnockbackForce;

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

            if (HandleImpact(other)) return;
        }

        private bool HandleImpact(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable == null) return true;


                damageable.ApplyDamage(currentDamage);
            }

            if (other.TryGetComponent<IKnockable>(out var knockable))
            {
                if (knockable == null) return true;
                Vector3 dir = (other.transform.position - transform.root.position);
                // Vector3 dir = (other.transform.position - characterOwnCollider.transform.position);
                dir.y = 0f;
                knockable.ApplyKnockback(currentKnockbackForce, dir);
            }

            ProcessImpactEvent(other);

            return false;
        }

        private void ProcessImpactEvent(Collider other)
        {
            Vector3 weaponPosition = transform.position;

            Vector3 hitPoint = other.ClosestPoint(weaponPosition);

            Vector3 hitDir = (hitPoint - weaponPosition).normalized;

            Vector3 normal = -hitDir;
            normal.Normalize();
            SurfaceType surface = surfaceDetection.GetSurfaceData(hitPoint);
            GameObject target = other.gameObject;

            //Calculate impact type by surface
            ImpactActionType impactType = ImpactCalculations(surface);

            var evt = new CharacterImpactActionEvent(this.gameObject, target, impactType, surface,
                normal);
            EventBus<CharacterImpactActionEvent>.Publish(evt);
        }

        private ImpactActionType ImpactCalculations(SurfaceType surface)
        {
            ImpactActionType impactType = ImpactActionType.generalImpact;
            switch (surface)
            {
                case SurfaceType.Flesh:
                    impactType = ImpactActionType.swordOnFlesh;
                    break;
                case SurfaceType.Metal:
                    impactType = ImpactActionType.swordOnMetal;
                    break;
                default:
                    return ImpactActionType.generalImpact;
                    break;
            }

            return impactType;
        }


        // public void SetAttackAttributes(float damageMultiplier, float knockbackForce, float attackDamage)
        // {
        //     this.currentAttackMultiplier = damageMultiplier;
        //     this.currentKnockbackForce = knockbackForce;
        //     this.currentDamage = attackDamage;
        // }
    }
}