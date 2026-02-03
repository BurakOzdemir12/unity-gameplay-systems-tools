using _Project.Systems._Core.EventBus.Interfaces;
using _Project.Systems.EnvironmentSystem.Weather.Enums;

namespace _Project.Systems.EnvironmentSystem.Weather.Events
{
    public struct WeatherChangedEvent : IEvent
    {
        public WeatherType CurrentWeatherType;

        public WeatherChangedEvent(WeatherType currentWeatherType)
        {
            CurrentWeatherType = currentWeatherType;
        }
    }
}