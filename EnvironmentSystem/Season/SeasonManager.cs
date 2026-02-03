using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Season.Enums;
using _Project.Systems.EnvironmentSystem.Season.Events;
using _Project.Systems.EnvironmentSystem.Season.ScriptableObjects;
using _Project.Systems.EnvironmentSystem.Time.Events;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Season
{
    public class SeasonManager : MonoBehaviour
    {
        [SerializeField] private SeasonConfigSo seasonData;
        private EventBinding<DayChangedEvent> dayChangedBinding;
        private SeasonType currentSeasonType;
        private int daysPassedInCurrentSeason;

        private void Awake()
        {
            currentSeasonType = SeasonType.Spring;
            daysPassedInCurrentSeason = 0;
        }

        private void OnEnable()
        {
            dayChangedBinding = new EventBinding<DayChangedEvent>(HandleDayChangedEvent);
            EventBus<DayChangedEvent>.Subscribe(dayChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<DayChangedEvent>.Unsubscribe(dayChangedBinding);
        }

        private void HandleDayChangedEvent(DayChangedEvent evt)
        {
            daysPassedInCurrentSeason++;
            if (daysPassedInCurrentSeason >= seasonData.daysPerSeason)
            {
                daysPassedInCurrentSeason = 0;
                SwitchToNextSeason();
            }
        }

        private void SwitchToNextSeason()
        {
            int nextSeasonIndex = ((int)currentSeasonType + 1) % 4;
            currentSeasonType = (SeasonType)nextSeasonIndex;
            Debug.Log("Season Changed to: " + currentSeasonType);
            EventBus<SeasonChangedEvent>.Publish(new SeasonChangedEvent(currentSeasonType));
        }
    }
}