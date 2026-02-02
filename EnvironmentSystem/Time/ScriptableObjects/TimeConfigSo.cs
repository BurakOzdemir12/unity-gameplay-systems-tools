using _Project.Systems.EnvironmentSystem.ScriptableObjects;
using _Project.Systems.EnvironmentSystem.Time.Enums;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Time.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewTimeConfig", menuName = "Scriptable Objects/Time/Time Config")]
    public class TimeConfigSo : ScriptableObject
    {
        public DivisionsOfDay defaultDivision;
        public float startHour;
        public float sunriseHour;
        public float sunsetHour;
        public float timeMultiplier;

    }
}