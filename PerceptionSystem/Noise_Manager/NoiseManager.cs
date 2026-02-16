using _Project.Systems.PerceptionSystem.Interfaces;
using _Project.Systems.PerceptionSystem.Structs;
using UnityEngine;

namespace _Project.Systems.PerceptionSystem.Noise_Manager
{
    public class NoiseManager : MonoBehaviour
    {
        public static NoiseManager Instance { get; private set; }

        // [SerializeField] private LayerMask listenerLayers;
        private readonly Collider[] hitBuffer = new Collider[10];

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void EmitNoise(Vector3 position, float radius, GameObject source, LayerMask listenerLayers)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(position, radius, hitBuffer, listenerLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                if (hitBuffer[i].TryGetComponent<INoiseListener>(out var listener))
                {
                    listener.OnNoiseDetected(new NoiseData(position, source, radius));
                }
            }
        }
    }
}