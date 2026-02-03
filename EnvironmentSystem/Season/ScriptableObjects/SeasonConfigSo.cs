using System.Collections.Generic;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Season.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SeasonDataSo", menuName = "Scriptable Objects/Season/Season Config")]
    public class SeasonConfigSo : ScriptableObject
    {
        public int daysPerSeason;

        public List<WeatherType> springWeathers;
        public List<WeatherType> summerWeathers;
        public List<WeatherType> autumnWeathers;
        public List<WeatherType> winterWeathers;
    }
}