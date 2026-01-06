using UnityEngine;

namespace _Project.Systems.ClimbingSystem.ScriptableObjects
{
    public abstract class VaultBaseSo : ClimbTypeDataSo
    {
        [Header("Vault Rules")] [Tooltip("If |localHit.x| <= threshold => considered CENTER vault.")] [SerializeField]
        protected float centerThreshold;

        protected void ComputeVaultFlags(in LedgeHitData hit, out Vector3 localHit, out bool isCenter, out bool mirror)
        {
            // local space relative to the hit object's transform
            localHit = hit.ForwardRayHitInfo.transform.InverseTransformPoint(hit.ForwardRayHitInfo.point);
            // Debug.Log(localHit + "x: " + localHit.x);
            // center window
            isCenter = Mathf.Abs(localHit.x) <= centerThreshold;

            // stable mirror rule: right side -> mirror true

            if ((localHit.z < 0 && localHit.x < -centerThreshold) || (localHit.z > 0 && localHit.x > centerThreshold))
            {
                mirror = true;
            }
            else
            {
                mirror = false;
            }
        }
    }
}