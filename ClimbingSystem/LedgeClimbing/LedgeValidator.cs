using UnityEngine;

namespace _Project.Systems.ClimbingSystem.LedgeClimbing
{
    public class LedgeValidator : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayers;

        #region Forward Raycast fields

        [Tooltip("Ray distance for forward Raycast")] [SerializeField]
        private float forwardRayLength;

        [Tooltip("Ray origin Vector for Forward Raycast")] [SerializeField]
        private Vector3 rayOriginOffset = new Vector3(0, 0.25f, 0);

        #endregion

        #region Height Raycast fields

        [Tooltip("Ray distance for calculation of height")] [SerializeField]
        private float heightRayLength;

        [Tooltip("Ray origin Vector for Height  Raycast")] [SerializeField]
        private Vector3 heightOriginOffset;

        #endregion

        //First Ray info for detection forward

        private LedgeHitData lastHitData;
        public LedgeHitData LastHitData => lastHitData;


        public LedgeHitData DetectLedge()
        {
            var hitData = new LedgeHitData();

            #region Forward Edge Raycast

            hitData.IsForwardRaycastHit = Physics.Raycast(transform.position + rayOriginOffset, transform.forward,
                out hitData.ForwardRayHitInfo,
                forwardRayLength,
                obstacleLayers);
            #endregion

            #region Height Edge Raycast

            if (hitData.IsForwardRaycastHit)
            {
                heightOriginOffset = hitData.ForwardRayHitInfo.point + Vector3.up * heightRayLength;
                hitData.IsHeightRaycastHit = Physics.Raycast(heightOriginOffset, Vector3.down,
                    out hitData.HeightRayHitInfo, heightRayLength,
                    obstacleLayers);
            }
            #endregion

            lastHitData = hitData;
            return hitData;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = lastHitData.IsForwardRaycastHit ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position + rayOriginOffset, transform.forward * forwardRayLength);

            Gizmos.color = lastHitData.IsHeightRaycastHit ? Color.green : Color.red;
            Gizmos.DrawRay(heightOriginOffset, Vector3.down * heightRayLength);
        }
    }
}

public struct LedgeHitData
{
    public bool IsForwardRaycastHit;
    public bool IsHeightRaycastHit;
    public RaycastHit ForwardRayHitInfo;
    public RaycastHit HeightRayHitInfo;
    
    public bool IsValidLedge => IsForwardRaycastHit && IsHeightRaycastHit;
}