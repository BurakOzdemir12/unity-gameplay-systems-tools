using System;
using System.Collections;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using _Project.Systems.EnvironmentSystem.Weather.Events;
using _Project.Systems.EnvironmentSystem.Weather.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Weather
{
    public class WeatherFXManager : MonoBehaviour
    {
        [SerializeField] private WeathersConfigSo weatherData;
        [SerializeField] private Camera mainCamera;

        [Space(10)] [SerializeField] private GameObject particlesContainer;

        [Header("Snow Weather Settings")] [SerializeField]
        private ParticleSystem snowParticles;

        private static readonly int SNOW_AMOUNT_ID = Shader.PropertyToID("_SnowAmount");
        [SerializeField] private float snowAccumulationSpeed = 0.1f;
        [SerializeField] private float snowMeltSpeed = 0.1f;
        private float currentSnowAmount = 0f;

        [Header("Rainy Weather Settings")] [SerializeField]
        private ParticleSystem rainParticles;
        //TODO create Wetness property id

        private EventBinding<WeatherChangedEvent> weatherChangedBinding;
        [Header("Current Weather")] private WeatherType currentWeatherType;

        //Coroutines
        private Coroutine snowAccumulationCoroutine;
        private Coroutine rainAccumulationCoroutine;

        private void Awake()
        {
            if (weatherData.rainParticlePrefab != null)
            {
                rainParticles = Instantiate(weatherData.rainParticlePrefab, particlesContainer.transform);
                rainParticles.transform.position = particlesContainer.transform.position;
                rainParticles.Stop();
            }

            if (weatherData.snowParticlePrefab != null)
            {
                snowParticles = Instantiate(weatherData.snowParticlePrefab, particlesContainer.transform);
                snowParticles.transform.position = particlesContainer.transform.position;
                snowParticles.Stop();
            }
        }
        private void OnEnable()
        {
            weatherChangedBinding = new EventBinding<WeatherChangedEvent>(HandleWeatherChangedEvent);
            EventBus<WeatherChangedEvent>.Subscribe(weatherChangedBinding);
        }


        private void OnDisable()
        {
            EventBus<WeatherChangedEvent>.Unsubscribe(weatherChangedBinding);
        }

        private void Start()
        {
            mainCamera = Camera.main;
            
            currentWeatherType = WeatherType.Clear;
            currentSnowAmount = 0;
        }

        private void Update()
        {
            HandleParticlePosition();

        }

        private void HandleWeatherChangedEvent(WeatherChangedEvent evt)
        {
            UpdateWeather(evt.CurrentWeatherType);
            HandleWeatherShaderValues(evt.CurrentWeatherType);
        }

        private void HandleWeatherShaderValues(WeatherType targetWeather)
        {
            float targetSpeed = (targetWeather == WeatherType.Snowy)
                ? snowAccumulationSpeed
                : snowMeltSpeed;
            float targetSnowAmount = (targetWeather == WeatherType.Snowy) ? 1f : 0f;

            if (snowAccumulationCoroutine != null)
            {
                StopCoroutine(snowAccumulationCoroutine);
            }

            snowAccumulationCoroutine = StartCoroutine(SnowProcessRoutine(targetSnowAmount, targetSpeed));
        }
        private IEnumerator SnowProcessRoutine(float targetSnowAmount, float targetSpeed)
        {
            while (!Mathf.Approximately(currentSnowAmount, targetSnowAmount))
            {

                currentSnowAmount = Mathf.MoveTowards(currentSnowAmount, targetSnowAmount,
                    targetSpeed * UnityEngine.Time.deltaTime);
                Shader.SetGlobalFloat(SNOW_AMOUNT_ID, currentSnowAmount);
                yield return null;
            }

            currentSnowAmount = targetSnowAmount;
            Shader.SetGlobalFloat(SNOW_AMOUNT_ID, currentSnowAmount);

            snowAccumulationCoroutine = null;
        }
        private void HandleParticlePosition()
        {
            if (!mainCamera) return;
            Vector3 targetPos = mainCamera.transform.position;
            targetPos.y += 10;
            particlesContainer.transform.position = targetPos;
        }
        private void UpdateWeather(WeatherType weatherType)
        {
            currentWeatherType = weatherType;
            switch (weatherType)
            {
                case WeatherType.Rainy:
                    if (rainParticles != null) rainParticles.Play();
                    if (snowParticles.isPlaying) snowParticles.Stop();
                    break;
                case WeatherType.Snowy:
                    if (snowParticles != null) snowParticles.Play();
                    if (rainParticles.isPlaying) rainParticles.Stop();
                    break;
                case WeatherType.Clear:
                    if (rainParticles != null && rainParticles.isPlaying) rainParticles.Stop();
                    if (snowParticles != null && snowParticles.isPlaying) snowParticles.Stop();
                    break;
            }
        }
    }
}