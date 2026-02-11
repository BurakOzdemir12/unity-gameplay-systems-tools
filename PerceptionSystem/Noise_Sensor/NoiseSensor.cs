using System;
using _Project.Systems.PerceptionSystem.Interfaces;
using _Project.Systems.PerceptionSystem.Structs;
using UnityEngine;

namespace _Project.Systems.PerceptionSystem.Noise_Sensor
{
    public class NoiseSensor : MonoBehaviour, INoiseListener
    {
        public event Action<NoiseData> OnNoiseHeard;

        public void OnNoiseDetected(NoiseData noiseData)
        {
            //TODO Raycast object check, walls, etc.
            //TODO Add noise filters 
            
            Debug.Log("Enemy heard player Noise");
            OnNoiseHeard?.Invoke(noiseData);
        }
    }
}