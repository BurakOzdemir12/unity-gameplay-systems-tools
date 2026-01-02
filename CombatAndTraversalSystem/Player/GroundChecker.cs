using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player
{
    public class GroundChecker : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.1f, 0);

        [Header("State")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private float distanceToGround;
        [SerializeField] private Vector3 groundNormal;

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

            // Using SphereCast for better coverage than Raycast
            if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
                {
                     isGrounded = false;
                     distanceToGround = float.MaxValue;
                     groundNormal = Vector3.up;
                     return;
                }

                isGrounded = true;
                distanceToGround = hit.distance;
                groundNormal = hit.normal;
            }
            else
            {
                isGrounded = false;
                distanceToGround = float.MaxValue;
                groundNormal = Vector3.up;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 origin = transform.position + offset;
            
            Gizmos.DrawWireSphere(origin, groundCheckRadius);
            Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance + groundCheckRadius));
            
            if (isGrounded)
            {
               Vector3 hitPos = origin + Vector3.down * distanceToGround;
               Gizmos.DrawWireSphere(hitPos, groundCheckRadius);
            }
        }
    }
}