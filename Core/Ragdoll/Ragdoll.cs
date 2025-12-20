using System;
using UnityEngine;

namespace _Project.Systems.Core.Ragdoll
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
            colliders = GetComponentsInChildren<Collider>(true);
            rigidbodies = GetComponentsInChildren<Rigidbody>(true);
            joints = GetComponentsInChildren<CharacterJoint>(true);
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

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Ragdoll"))
                {
                    collider.enabled = isRagdoll;
                }
            }

            foreach (Rigidbody rb in rigidbodies)
            {
                if (rb.gameObject.CompareTag("Ragdoll"))
                {
                    rb.isKinematic =
                        !isRagdoll; // if isKinematic is false, means that physics does actualyy effects the rb
                    rb.useGravity = isRagdoll;
                }
            }

            foreach (CharacterJoint joint in joints)
            {
                if (joint.gameObject.CompareTag("Ragdoll"))
                {
                    joint.enableCollision = isRagdoll;
                }
            }
        }
    }
}