using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Weather.ScriptableObjects
{
    [CreateAssetMenu(fileName = "WeatherDataSo", menuName = "Scriptable Objects/Weather/WEather Config")]
    public class WeathersConfigSo : ScriptableObject
    {
        public ParticleSystem rainParticlePrefab;
        public ParticleSystem snowParticlePrefab;
        
        public Skybox skybox;
    }
}