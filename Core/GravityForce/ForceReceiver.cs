using _Project.Systems.Core.GravityForce.Interfaces;
using UnityEngine;

namespace _Project.Systems.Core.GravityForce
{
    public class ForceReceiver : MonoBehaviour, IKnockable
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private float dragTime = 0.2f;

        private float verticalVelocity;
        private Vector3 impact;
        private Vector3 dampingVelocity;

        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }
        }

        public Vector3 Movement => impact + Vector3.up * verticalVelocity; // 0,5,0 etc.

        void Update()
        {
            if (verticalVelocity < 0f && controller.isGrounded)
            {
                verticalVelocity = Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }

            impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, dragTime);
            // if (agent != null)
            // {
            //     if (impact.sqrMagnitude < 0.2f * 0.2f)
            //     {
            //         impact = Vector3.zero;
            //         agent.enabled = true;
            //     }
            // }

        }

        public void AddForce(Vector3 force)
        {
            impact += force;
        }

        public void ApplyKnockback(float knockbackForce, Vector3 forceDirection)
        {
            Vector3 dir = forceDirection;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;
            AddForce(dir.normalized * knockbackForce);
        }

        public void ApplyJumpForce(float force)
        {
            verticalVelocity += force;
        }
    }
}