using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Weather.Events;
using TMPro;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Weather.UI
{
    public class WeatherDisplayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weatherText;
        private EventBinding<WeatherChangedEvent> weatherChangedBinding;

        private void OnEnable()
        {
            weatherChangedBinding = new EventBinding<WeatherChangedEvent>(HandleWeatherChangedEvent);
            EventBus<WeatherChangedEvent>.Subscribe(weatherChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<WeatherChangedEvent>.Unsubscribe(weatherChangedBinding);
        }
        private void HandleWeatherChangedEvent(WeatherChangedEvent evt)
        {
            weatherText.text = evt.CurrentWeatherType.ToString();
        }

    }
}