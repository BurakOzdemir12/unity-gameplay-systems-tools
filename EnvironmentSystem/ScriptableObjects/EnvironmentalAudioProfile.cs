using _Project.Systems.EnvironmentSystem.Time.Enums;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AudioProfile", menuName = "Scriptable Objects/Audio Profile/Env Audio Profile")]
    public class EnvironmentalAudioProfile : ScriptableObject
    {
        [Header("Audio Settings by Time")] public AudioClip clearDaySound;
        public AudioClip clearNightSound;
        [Header("Audio Settings by Weather")] public AudioClip rainSound;
        public AudioClip hardRainSound;
        public AudioClip snowSound;
        [Header("Audio Settings by Location")] public AudioClip forestSound;
        public AudioClip caveSound;


        public AudioClip GetEnvAudio(DivisionsOfDay division, WeatherType weatherType)
        {
            if (weatherType == WeatherType.Rainy) return rainSound;
            if (weatherType == WeatherType.Snowy) return snowSound;

            return division switch
            {
                DivisionsOfDay.Morning or DivisionsOfDay.Afternoon => clearDaySound,
                DivisionsOfDay.Evening or DivisionsOfDay.Night => clearNightSound,
                _ => clearDaySound
            };
        }

        private AudioClip GetRandom(AudioClip[] clips)
        {
            return clips.Length == 0 ? null : clips[Random.Range(0, clips.Length)];
        }
    }
}