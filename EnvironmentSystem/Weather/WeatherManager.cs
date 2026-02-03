using System;
using System.Collections.Generic;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Season.Enums;
using _Project.Systems.EnvironmentSystem.Season.Events;
using _Project.Systems.EnvironmentSystem.Season.ScriptableObjects;
using _Project.Systems.EnvironmentSystem.Time.Events;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using _Project.Systems.EnvironmentSystem.Weather.Events;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Systems.EnvironmentSystem.Weather
{
    public class WeatherManager : MonoBehaviour
    {
        [SerializeField] private SeasonConfigSo seasonData;

        private EventBinding<SeasonChangedEvent> seasonChangedBinding;
        private EventBinding<TimeChangedEvent> timeChangedBinding;

        private WeatherType currentWeatherType;
        private SeasonType currentSeasonType;

        private List<WeatherType> availableWeatherTypes = new List<WeatherType>();

        [SerializeField] private int maxRainDuration;
        [SerializeField] private int maxWeatherDuration;
        private int currentWeatherDuration;

        [SerializeField] private Button setSnowyButton;
        [SerializeField] private Button setRainyButton;
        [SerializeField] private Button setClearButton;

        private void Awake()
        {
            currentWeatherType = WeatherType.Clear;
            currentSeasonType = SeasonType.Spring;

            availableWeatherTypes = seasonData.springWeathers;
            currentWeatherDuration = 0;
        }

        private void OnEnable()
        {
            seasonChangedBinding = new EventBinding<SeasonChangedEvent>(HandleSeasonChangedEvent);
            EventBus<SeasonChangedEvent>.Subscribe(seasonChangedBinding);

            timeChangedBinding = new EventBinding<TimeChangedEvent>(HandleTimeChangedEvent);
            EventBus<TimeChangedEvent>.Subscribe(timeChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<SeasonChangedEvent>.Unsubscribe(seasonChangedBinding);
            EventBus<TimeChangedEvent>.Unsubscribe(timeChangedBinding);
        }


        private void Start()
        {
            //Testing
            if (setRainyButton != null)
                setRainyButton.onClick.AddListener(() =>
                    SetWeather(WeatherType.Rainy));
            if (setSnowyButton != null)
                setSnowyButton.onClick.AddListener(() =>
                    SetWeather(WeatherType.Snowy));
            if (setClearButton != null)
                setClearButton.onClick.AddListener(ForceClearWeather);
        }

        private void HandleSeasonChangedEvent(SeasonChangedEvent evt)
        {
            currentSeasonType = evt.CurrentSeasonType;
            UpdateWeatherTypesBasedOnSeason();

            DecideWeather();
        }

        private void UpdateWeatherTypesBasedOnSeason()
        {
            switch (currentSeasonType)
            {
                case SeasonType.Spring:
                    availableWeatherTypes = seasonData.springWeathers;
                    break;
                case SeasonType.Summer:
                    availableWeatherTypes = seasonData.summerWeathers;
                    break;
                case SeasonType.Autumn:
                    availableWeatherTypes = seasonData.autumnWeathers;
                    break;
                case SeasonType.Winter:
                    availableWeatherTypes = seasonData.winterWeathers;
                    break;
            }
        }

        private void HandleTimeChangedEvent(TimeChangedEvent evt)
        {
            currentWeatherDuration++;
            if (IsPrecipitation(currentWeatherType) && currentWeatherDuration > maxRainDuration)
            {
                ForceClearWeather();
                return;
            }

            if (UnityEngine.Random.value < 0.1f && currentWeatherDuration > maxWeatherDuration)
            {
                DecideWeather();
            }
        }

        private void ForceClearWeather()
        {
            SetWeather(WeatherType.Clear);
        }

        private void SetWeather(WeatherType newWeather)
        {
            if (currentWeatherType != newWeather)
            {
                currentWeatherType = newWeather;
                currentWeatherDuration = 0;
                Debug.Log($"Weather Changed: {currentWeatherType}");
                EventBus<WeatherChangedEvent>.Publish(new WeatherChangedEvent(currentWeatherType));
            }
        }

        private void DecideWeather()
        {
            if (availableWeatherTypes == null || availableWeatherTypes.Count == 0) return;

            WeatherType newWeather =
                availableWeatherTypes[UnityEngine.Random.Range(0, availableWeatherTypes.Count)];

            if (currentWeatherType != newWeather)
            {
                currentWeatherType = newWeather;
                Debug.Log($"Weather Changed: {currentWeatherType}");

                EventBus<WeatherChangedEvent>.Publish(new WeatherChangedEvent(currentWeatherType));
            }

            currentWeatherDuration = 0;
        }

        private bool IsPrecipitation(WeatherType type)
        {
            return type == WeatherType.Rainy || type == WeatherType.Snowy; // || type == WeatherType.Stormy;
        }

        private void OnDestroy()
        {
            if (setRainyButton != null) setRainyButton.onClick.RemoveAllListeners();
            if (setSnowyButton != null) setSnowyButton.onClick.RemoveAllListeners();
            if (setClearButton != null) setClearButton.onClick.RemoveAllListeners();
        }
    }
}