using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.LedgeClimbing
{
    public class LedgeValidator : MonoBehaviour
    {
        [Header("Ledge Detection")] [SerializeField]
        private Transform eyeLevel;

        [SerializeField] private float ledgeDetectionForwardMax = 1.0f;
        [SerializeField] private float ledgeInwardOffset = 0.5f;

        [Header("Height Validation")] [SerializeField]
        private float playerMaxClimbHeight = 2.0f;

        [SerializeField] private float minClimbHeight = 0.5f;

        [Header("Layers")] [Tooltip("Climbable Layers")] [SerializeField]
        private LayerMask climbableObjectLayers;

        [Tooltip("for detection grounds for calculation height of object")] [SerializeField]
        private LayerMask groundLayers;

        [Header("Fit Check")] [Tooltip("Will character fits?")] [SerializeField]
        private float characterRadius = 0.5f;

        [SerializeField] private float characterHeight = 1.8f;
        [SerializeField] private LayerMask fitDetectionLayers;


        public struct LedgeData
        {
            public bool isValid;
            public Vector3 surfacePoint;
            public Vector3 groundPoint;
            public float height;
            public Vector3 wallNormal;
        }

        public LedgeData currentLedge;
        public LedgeData lockedLedge;
        public bool isLocked;

        // Debug & State
        [Header("Debug Info")] [SerializeField]
        private bool canClimbLedge;

        public bool CanClimbLedge => canClimbLedge;

        [SerializeField] private float detectedHeight;
        public float DetectedHeight => detectedHeight;

        private Vector3 _debugRayStart;
        private Vector3 _debugSurfacePoint;
        private Vector3 _debugGroundPoint;

        private void Update()
        {
            if (isLocked)
            {
                canClimbLedge = lockedLedge.isValid;
                detectedHeight = lockedLedge.height;
                _debugSurfacePoint = lockedLedge.surfacePoint;
                _debugGroundPoint = lockedLedge.groundPoint;
                return;
            }

            CheckLedge();
        }

        public bool LockCurrentLedge()
        {
            // if (isLocked) return lockedLedge.isValid;

            if (!currentLedge.isValid)
            {
                return false;
            }

            lockedLedge = currentLedge;
            isLocked = true;
            return true;
        }

        public void ClearLock()
        {
            isLocked = false;
            lockedLedge = default;
        }

        private void CheckLedge()
        {
            canClimbLedge = false;
            detectedHeight = 0f;

            currentLedge = default;
            currentLedge.isValid = false;

            if (!Physics.Raycast(
                    eyeLevel.position,
                    transform.forward,
                    out RaycastHit forwardHit,
                    ledgeDetectionForwardMax,
                    climbableObjectLayers))
            {
                return;
            }

            Vector3 rayStartPos = forwardHit.point + (transform.forward * ledgeInwardOffset) +
                                  (Vector3.up * (playerMaxClimbHeight + 0.5f));
            _debugRayStart = rayStartPos;

            if (!Physics.Raycast(
                    rayStartPos,
                    Vector3.down,
                    out RaycastHit surfaceHit,
                    playerMaxClimbHeight + 1.0f,
                    climbableObjectLayers))
            {
                return;
            }

            _debugSurfacePoint = surfaceHit.point;

            if (!Physics.Raycast(
                    surfaceHit.point + Vector3.up * 0.1f,
                    Vector3.down,
                    out RaycastHit groundHit,
                    20f,
                    groundLayers))
            {
                return;
            }

            _debugGroundPoint = groundHit.point;

            float objectHeightAboveGround = surfaceHit.point.y - groundHit.point.y;
            detectedHeight = objectHeightAboveGround;

            if (objectHeightAboveGround > playerMaxClimbHeight || objectHeightAboveGround < minClimbHeight)
            {
                return;
            }

            if (!IsPlayerCanFitSpace(surfaceHit.point))
            {
                return;
            }

            canClimbLedge = true;
            currentLedge.isValid = true;
            currentLedge.surfacePoint = surfaceHit.point;
            currentLedge.groundPoint = groundHit.point;
            currentLedge.height = objectHeightAboveGround;
            currentLedge.wallNormal = surfaceHit.normal;
        }

        private bool IsPlayerCanFitSpace(Vector3 targetPos)
        {
            float padding = 0.05f;

            Vector3 pointBottom = targetPos + Vector3.up * (characterRadius + padding);

            Vector3 pointTop = targetPos + Vector3.up * (characterHeight - characterRadius - padding);

            bool hitsSomething = Physics.CheckCapsule(
                pointBottom,
                pointTop,
                characterRadius,
                fitDetectionLayers,
                QueryTriggerInteraction.Ignore);

            return !hitsSomething;
        }

        private void OnDrawGizmos()
        {
            if (eyeLevel == null) return;

            // 1. Forward Ray Visual
            Gizmos.color = canClimbLedge ? Color.green : Color.red;
            Gizmos.DrawRay(eyeLevel.position, transform.forward * ledgeDetectionForwardMax);

            if (_debugSurfacePoint != Vector3.zero)
            {
                // 2. Downward Check Visual (Ledge Top Finding)
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_debugRayStart, _debugSurfacePoint);
                Gizmos.DrawWireSphere(_debugSurfacePoint, 0.1f);

                // 3. Ground Check Visual
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(_debugSurfacePoint, _debugGroundPoint);
                Gizmos.DrawWireSphere(_debugGroundPoint, 0.1f);

                // 4. Fit Check Visual
                if (canClimbLedge)
                {
                    Gizmos.color = Color.darkGreen;
                    Vector3 center = _debugSurfacePoint + Vector3.up * (characterHeight / 2f);
                    Gizmos.DrawWireCube(center, new Vector3(characterRadius * 2, characterHeight, characterRadius * 2));
                }
            }
        }
    }
}