using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.Gravity_Force
{
    public class ForceReceiver : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;

        private float verticalVelocity;

        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }
        }

        public Vector3 Movement => Vector3.up * verticalVelocity; // 0,5,0 etc.

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
        }
    }
}