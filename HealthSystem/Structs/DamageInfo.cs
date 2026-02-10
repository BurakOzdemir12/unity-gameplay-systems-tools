using UnityEngine;

namespace _Project.Systems.HealthSystem.Structs
{
    public struct DamageInfo
    {
        public float Damage;
        public GameObject TargetRoot;
        public GameObject SourceObject;

        // public Collider HitCollider;
        //IsCritical
        //HitPoint

        public DamageInfo(float damage, GameObject targetRoot, GameObject sourceObject)
        {
            Damage = damage;
            TargetRoot = targetRoot;
            SourceObject = sourceObject;
        }
    }
}