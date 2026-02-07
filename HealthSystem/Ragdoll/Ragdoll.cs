using System.Linq;
using UnityEngine;

namespace _Project.Systems.HealthSystem.Ragdoll
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController controller;
        [SerializeField] private bool testRagdoll = false;
        // [SerializeField] private Transform RagdollRoot;

        private Rigidbody[] rigidbodies;
        private CharacterJoint[] joints;
        private Collider[] colliders;

        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider>(true)
                .Where(c => c.CompareTag("Ragdoll")).ToArray();
            rigidbodies = GetComponentsInChildren<Rigidbody>(true)
                .Where(rb => rb.CompareTag("Ragdoll")).ToArray();
            joints = GetComponentsInChildren<CharacterJoint>(true)
                .Where(j => j.CompareTag("Ragdoll")).ToArray();
        }

        private void Start()
        {
            ToggleRagdoll(false);
        }

        private void Update()
        {
            if (testRagdoll)
                ToggleRagdoll(true);
        }

        public void ToggleRagdoll(bool isRagdoll)
        {
            animator.enabled = !isRagdoll;
            controller.enabled = !isRagdoll;

            foreach (Collider col in colliders)
            {
                if (!col) continue;
                if (col.gameObject.CompareTag("Ragdoll"))
                {
                    col.enabled = isRagdoll;
                }
            }

            foreach (Rigidbody rb in rigidbodies)
            {
                if (!rb) continue;
                if (rb.gameObject.CompareTag("Ragdoll"))
                {
                    rb.isKinematic =
                        !isRagdoll; // if isKinematic is false, means that physics does actualyy effects the rb
                    rb.useGravity = isRagdoll;
                }
            }

            foreach (CharacterJoint joint in joints)
            {
                if (!joint) continue;
                if (joint.gameObject.CompareTag("Ragdoll"))
                {
                    joint.enableCollision = isRagdoll;
                }
            }
        }
    }
}