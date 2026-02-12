using System;
using _Project.Systems.PerceptionSystem.Interfaces;
using _Project.Systems.PerceptionSystem.Structs;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Systems.PerceptionSystem.Noise_Sensor
{
    public class NoiseSensor : MonoBehaviour, INoiseListener
    {
        [Header("Noise Sensor Settings")] [SerializeField]
        private Vector3 linecastOffset;

        [SerializeField] private LayerMask occlusionLayers;
        [SerializeField] private string[] ignoredOcclusionTags;

        [Space(5)]
        [Tooltip("How much sound remains after passing through an obstacle? (1 = Full, 0 = None)")]
        [Range(0f, 1f)]
        [SerializeField]
        private float defaultOcclusionDamping = 1f;

        public event Action<NoiseData> OnNoiseHeard;

        private RaycastHit hitInfo;

        private Vector3 incomeNoisePos;

        //DEbug
        private float debugDistance;
        private float debugEffectiveRadius;


        public void OnNoiseDetected(NoiseData noiseData)
        {
            Vector3 origin = transform.TransformPoint(linecastOffset);

            bool isHit = Physics.Linecast(origin, noiseData.Position, out hitInfo,
                occlusionLayers);

            incomeNoisePos = noiseData.Position;
            float distanceToSource = Vector3.Distance(origin, incomeNoisePos);

            float dampingFactor = 1f;

            // ? Remember you can reduce noises at different levels by different object types => Tags
            // hitInfo.transform.gameObject.CompareTag(STONE_WALL);
            //? But this is the best simple way to do it =>

            if (isHit)
            {
                bool isIgnoredObstacle = false;
                foreach (var tagToCheck in ignoredOcclusionTags)
                {
                    if (hitInfo.transform.gameObject.CompareTag(tagToCheck))
                    {
                        isIgnoredObstacle = true;
                        break;
                    }
                }

                if (!isIgnoredObstacle)
                {
                    //? If obstacle is not ignored, reduce its volume by dampingFactor 
                    //? if defaultOcclusionDamping = .4f => 60% reduction
                    dampingFactor = defaultOcclusionDamping;
                }

                float newEffectiveRadius = noiseData.Range * dampingFactor;
                //debug
                debugDistance = distanceToSource;
                debugEffectiveRadius = newEffectiveRadius;
                if (distanceToSource <= newEffectiveRadius)
                {
                    Debug.Log($"Enemy heard noise! Dist: {distanceToSource:F1}, EffRadius: {newEffectiveRadius:F1}");
                    OnNoiseHeard?.Invoke(noiseData);
                }
                else
                {
                    Debug.Log(
                        $"Noise blocked by wall. Dist: {distanceToSource:F1} > EffRadius: {newEffectiveRadius:F1}");
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.TransformPoint(linecastOffset);

            Gizmos.color = Color.saddleBrown;
            Gizmos.DrawLine(origin, incomeNoisePos);

            if (debugEffectiveRadius > 0)
            {
                Gizmos.color = debugDistance <= debugEffectiveRadius ? Color.green : Color.red;
                Gizmos.DrawWireSphere(origin, debugEffectiveRadius);
            }
        }
    }
}