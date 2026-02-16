using System;
using _Project.Systems.PerceptionSystem.Interfaces;
using _Project.Systems.PerceptionSystem.Noise_Manager;
using UnityEngine;

namespace _Project.Systems.SharedGameplay
{
    public class Throwable : MonoBehaviour
    {
        //TODO Create Scriptable Object for objects so we can change the noise radius, layers and other settings
        //TODO Create Base monobehaviour for collision control and noise - Sound fx generation
        [Header("Settings")] [Tooltip("Who is gonna heard the noise => enemy")] [SerializeField]
        private LayerMask listenerLayers;

        [Tooltip("Noise radius when rock hit")] [SerializeField]
        private float baseNoiseRadius = 5f;

        [Tooltip("needs to minimum velocity for make sound")] [SerializeField]
        private float minVelocityToMakeNoise = 1f;

        [Tooltip("Object will be destroyed after this time")] [SerializeField]
        private float destroyTime = 15f;

        private Vector3 debugFirstContactPoint;
        private bool hasAlreadyCollided = false;
        private float timer;

        private void Start()
        {
            Destroy(gameObject, destroyTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasAlreadyCollided) return;

            float velocity = collision.relativeVelocity.magnitude;

            if (velocity > minVelocityToMakeNoise)
            {
                float finalRadius = baseNoiseRadius * Mathf.Clamp(velocity * 0.5f, 0.5f, 2f);

                NoiseManager.Instance.EmitNoise(collision.contacts[0].point, finalRadius, gameObject, listenerLayers);
#if UNITY_EDITOR
                debugFirstContactPoint = collision.contacts[0].point;
#endif
                hasAlreadyCollided = true;
                // Debug.Log($"{gameObject.name} voice ! Radius: {finalRadius}");
            }

            CheckInteraction(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckInteraction(other);
        }

        //? if it is encountered with any Character that has IListener, it'll destroy itself
        private void CheckInteraction(Collider other)
        {
            if (((1 << other.gameObject.layer) & listenerLayers) == 0) return;
            Transform rootTarget = other.transform.root;
            if (rootTarget.TryGetComponent<INoiseListener>(out var listener))
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(debugFirstContactPoint, baseNoiseRadius);
        }
    }
}