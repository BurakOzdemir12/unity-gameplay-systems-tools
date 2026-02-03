using System;
using System.Globalization;
using _Project.Systems.EnvironmentSystem.Time.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Project.Systems.EnvironmentSystem.Time
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [SerializeField] private TimeService timeService;
        public TimeService TimeService => timeService;

        [SerializeField] private TimeConfigSo timeConfig;
        public TimeConfigSo TimeConfig => timeConfig;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            timeService = new TimeService(timeConfig);
        }

        private void Update()
        {
            UpdateTimeOfDay();

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                timeConfig.timeMultiplier *= 2;
            }

            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                timeConfig.timeMultiplier /= 2;
            }

            Debug.Log("Day: " + timeService.CurrentTime.Day + " Time: ");
        }

        private void UpdateTimeOfDay()
        {
            timeService.UpdateTime(UnityEngine.Time.deltaTime);
        }


        //TODO Dont glue scripts each other use services for get CurrentTime
        // DateTime now = ServiceLocator.Get<TimeService>().CurrentTime;
    }
}