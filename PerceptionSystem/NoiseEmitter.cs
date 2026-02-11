using System;
using System.Collections.Generic;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems.MovementSystem.Events;
using _Project.Systems.PerceptionSystem.Interfaces;
using _Project.Systems.PerceptionSystem.Structs;
using UnityEngine;

namespace _Project.Systems.PerceptionSystem
{
    public class NoiseEmitter : MonoBehaviour
    {
        [SerializeField] private LayerMask listenerLayers;
        [SerializeField] private Vector3 emissionPoint;
        [SerializeField] private float emissionRadius;

        [SerializeField] private int bufferMax = 4;
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
            //? its basically calculates Sound Radius -> Calculates radius based on character's movement type'
            float soundLevel = CalculateRadius(evt.Type, evt.ActionTag);
            EmitNoise();
        }

        private float CalculateRadius(TraversalType evtType, string actionTag)
        {
            switch (evtType)
            {
                case TraversalType.Footstep when actionTag == "":
                    //TODO You have to separate traversal types
                    //TODO otherwise it will be too difficult to calculate with action tags

                    return 5f; //TODO creat SerializeField for thees
                case TraversalType.Land:
                    return 7f;
                case TraversalType.Jump:
                    return 6f;
                default: return 0;
            }
        }

        private void EmitNoise()
        {
            Vector3 origin = transform.TransformPoint(emissionPoint);
            var hitCount = Physics.OverlapSphereNonAlloc(origin, emissionRadius, hitBuffer, listenerLayers,
                QueryTriggerInteraction.Ignore);
            buffersForEmittingNoise.Clear();
            for (int i = 0; i < hitCount; ++i)
            {
                Collider col = hitBuffer[i];

                if (buffersForEmittingNoise.Add(col))
                {
                    col.TryGetComponent<INoiseListener>(out var listener);
                    listener?.OnNoiseDetected(new NoiseData(transform.position, transform.root.gameObject,
                        emissionRadius));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gold;
            Vector3 origin = Application.isPlaying
                ? transform.TransformPoint(emissionPoint)
                : transform.position + emissionPoint;
            Gizmos.DrawWireSphere(origin, emissionRadius);
        }

        private void OnDisable()
        {
            EventBus<CharacterTraversalEvent>.Unsubscribe(traversalEventBinding);
        }
    }
}