using UnityEngine;

namespace _Project.Systems.Core.GravityForce
{
    public class ForceReceiver : MonoBehaviour
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
        }

        public void AddForce(Vector3 force)
        {
            impact += force;
        }
    }
}