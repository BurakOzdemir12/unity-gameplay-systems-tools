using System;
using System.Collections.Generic;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems.MovementSystem.Events;
using _Project.Systems.PerceptionSystem.Enums;
using _Project.Systems.PerceptionSystem.Interfaces;
using _Project.Systems.PerceptionSystem.Structs;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems.PerceptionSystem
{
    public class NoiseEmitter : MonoBehaviour
    {
        [Header("Noise Emitter Settings")] [SerializeField]
        private LayerMask listenerLayers;

        [SerializeField] private Vector3 emissionPoint;
        [SerializeField] private float emissionRadius;
        [SerializeField] private int bufferMax = 4;

        [Header("Noise Intensity Settings")] [Range(0f, 1f)] [SerializeField]
        private float walkNoiseIntensity = 0.5f;

        [Range(0f, 1f)] [SerializeField] private float runNoiseIntensity = 1f;
        [Range(0f, 1f)] [SerializeField] private float sprintNoiseIntensity = 1.5f;
        [Range(0f, 1f)] [SerializeField] private float jumpNoiseIntensity = 2f;
        [Range(0f, 1f)] [SerializeField] private float landNoiseIntensity = 0.5f;
        [Range(0f, 1f)] [SerializeField] private float swimNoiseIntensity = 0.3f;

        [SerializeField] private float calculatedIntensity;


        private HashSet<Collider> buffersForEmittingNoise;
        private Collider[] hitBuffer;
        private EventBinding<CharacterTraversalEvent> traversalEventBinding;

        private void OnEnable()
        {
            traversalEventBinding = new EventBinding<CharacterTraversalEvent>(HandleTraversalEvent);
            EventBus<CharacterTraversalEvent>.Subscribe(traversalEventBinding);
        }


        private void Start()
        {
            buffersForEmittingNoise = new HashSet<Collider>(bufferMax);
            hitBuffer = new Collider[bufferMax];
        }

        private void HandleTraversalEvent(CharacterTraversalEvent evt)
        {
            //? it basically calculates Sound Radius -> Calculates radius based on character's traversal type'
            if (evt.Source != this.gameObject) return;

            float soundIntensity = CalculateRadius(evt.Type, evt.ActionTag);
            EmitNoise(evt.Position, soundIntensity);
        }

        private float CalculateRadius(TraversalType evtType, string actionTag)
        {
            switch (evtType)
            {
                case TraversalType.Walk:
                    return calculatedIntensity = walkNoiseIntensity;
                case TraversalType.Run:
                    return calculatedIntensity = runNoiseIntensity;
                case TraversalType.Sprint:
                    return calculatedIntensity = sprintNoiseIntensity;
                case TraversalType.Land:
                    return calculatedIntensity = landNoiseIntensity;
                case TraversalType.Jump:
                    return calculatedIntensity = jumpNoiseIntensity;
                case TraversalType.Swim:
                    return calculatedIntensity = swimNoiseIntensity;
                case TraversalType.Crouch:
                    return calculatedIntensity = 0;
                case TraversalType.Crawl:
                    return calculatedIntensity = 0;
                case TraversalType.Climb:
                    return calculatedIntensity = 0;
                case TraversalType.Slide:
                    return calculatedIntensity = 0;
                default: return calculatedIntensity = 0;
            }
        }

        private void EmitNoise(Vector3 emissionPosition, float intensity)
        {
            // ! emissionPosition can be use for other emitters like arrow or magic spell hit location
            // ! but for smetimes it gives buggy results because the position may be under or inside the wall'
            Vector3 origin = transform.TransformPoint(emissionPoint);
            float finalRadius = intensity * emissionRadius;

            var hitCount = Physics.OverlapSphereNonAlloc(origin, finalRadius, hitBuffer, listenerLayers,
                QueryTriggerInteraction.Ignore);

            buffersForEmittingNoise.Clear();
            for (int i = 0; i < hitCount; ++i)
            {
                Collider col = hitBuffer[i];

                if (buffersForEmittingNoise.Add(col))
                {
                    col.TryGetComponent<INoiseListener>(out var listener);
                    listener?.OnNoiseDetected(new NoiseData(origin, transform.root.gameObject,
                        finalRadius, PerceptionType.Hostile));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gold;
            Vector3 origin = Application.isPlaying
                ? transform.TransformPoint(emissionPoint)
                : transform.position + emissionPoint;
            Gizmos.DrawWireSphere(origin, emissionRadius * calculatedIntensity);
        }

        private void OnDisable()
        {
            EventBus<CharacterTraversalEvent>.Unsubscribe(traversalEventBinding);
        }
    }
}