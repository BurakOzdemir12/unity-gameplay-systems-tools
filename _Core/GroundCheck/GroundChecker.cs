using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _Project.Systems._Core.GroundCheck
{
    public class GroundChecker : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.1f, 0);
        [SerializeField] private Vector3 leftOffset = new Vector3(0, 0.1f, 0);
        [SerializeField] private Vector3 rightOffset = new Vector3(0, 0.1f, 0);

        [Header("State")] [SerializeField] private bool isGrounded;
        [SerializeField] private float distanceToGround;
        [SerializeField] private Vector3 groundNormal;

        [Header("Capsule Or Sphere")] [SerializeField]
        private bool useCapsule;

        [Header("Foot Bones")] [SerializeField]
        private Transform leftFoot;

        [SerializeField] private Transform rightFoot;

        public bool IsGrounded => isGrounded;
        public float DistanceToGround => distanceToGround;
        public Vector3 GroundNormal => groundNormal;

        private void Update()
        {
            CheckGround();
        }

        [SerializeField] private float maxSlopeAngle = 45f;

        private void CheckGround()
        {
            Vector3 origin = transform.position + offset;
            RaycastHit hit;
            //I Set two option for ground check Capsule Cast or Sphere Cast
            switch (useCapsule)
            {
                case true:
                {
                    Vector3 point1 = transform.position;
                    Vector3 point2 = transform.position + offset;
                    if (Physics.CapsuleCast(point1, point2, groundCheckRadius,
                            Vector3.down, out hit, groundCheckDistance, groundLayer,
                            QueryTriggerInteraction.Ignore))
                    {
                        if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
                        {
                            FinalConclusion(false, float.MaxValue, Vector3.up);
                            return;
                        }

                        FinalConclusion(true, hit.distance, hit.normal);
                    }
                    else
                    {
                        FinalConclusion(false, float.MaxValue, Vector3.up);
                    }

                    break;
                }
                case false:
                {
                    Vector3 leftOrigin = leftFoot.position + leftOffset;
                    Vector3 rightOrigin = rightFoot.position + rightOffset;
                    // Using SphereCast for better coverage than Raycast

                    if (Physics.SphereCast(leftOrigin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance,
                            groundLayer)
                        ||
                        Physics.SphereCast(rightOrigin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance,
                            groundLayer))
                    {
                        if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
                        {
                            FinalConclusion(false, float.MaxValue, Vector3.up);
                            return;
                        }

                        FinalConclusion(true, hit.distance, hit.normal);
                    }
                    else
                    {
                        FinalConclusion(false, float.MaxValue, Vector3.up);
                    }

                    break;
                }
            }
        }

        private void FinalConclusion(bool grounded, float max, Vector3 dir)
        {
            this.isGrounded = grounded;
            distanceToGround = max;
            groundNormal = dir;
        }

        private void CalculateCapsulePoints(out Vector3 p1, out Vector3 p2)
        {
            float distanceToPoints = (offset.y / 2f) - groundCheckRadius;

            Vector3 center = transform.position + new Vector3(0, 1, 0);
            p1 = center + Vector3.up * distanceToPoints;
            p2 = center - Vector3.up * distanceToPoints;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            // Vector3 origin = transform.position + offset;
            Vector3 leftOrigin = leftFoot.position + leftOffset;
            Vector3 rightOrigin = rightFoot.position + rightOffset;
            if (useCapsule)
            {
                Vector3 point1 = transform.position;
                Vector3 point2 = transform.position + offset;
                CalculateCapsulePoints(out point1, out point2);
                Gizmos.DrawWireSphere(point1, groundCheckRadius);
                Gizmos.DrawWireSphere(point2, groundCheckRadius);

                Gizmos.DrawLine(point1 + Vector3.right * groundCheckRadius, point2 + Vector3.right * groundCheckRadius);
                Gizmos.DrawLine(point1 - Vector3.right * groundCheckRadius, point2 - Vector3.right * groundCheckRadius);
                Gizmos.DrawLine(point1 + Vector3.forward * groundCheckRadius,
                    point2 + Vector3.forward * groundCheckRadius);
                Gizmos.DrawLine(point1 - Vector3.forward * groundCheckRadius,
                    point2 - Vector3.forward * groundCheckRadius);
            }
            else
            {
                // Gizmos.DrawSphere(origin, groundCheckRadius);
                Gizmos.DrawSphere(leftOrigin, groundCheckRadius);
                Gizmos.DrawSphere(rightOrigin, groundCheckRadius);
            }

            // Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance + groundCheckRadius));
            Gizmos.DrawLine(leftOrigin, leftOrigin + Vector3.down * (groundCheckDistance + groundCheckRadius));
            Gizmos.DrawLine(rightOrigin, rightOrigin + Vector3.down * (groundCheckDistance + groundCheckRadius));
        }
    }
}