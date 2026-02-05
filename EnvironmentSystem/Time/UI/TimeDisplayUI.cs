using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Time.Events;
using TMPro;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Time.UI
{
    public class TimeDisplayUI : MonoBehaviour
    {
        private EventBinding<DayChangedEvent> dayChangedBinding;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dayText;

        private TimeService timeService;
        private int lastMinute = -1;

        private void OnEnable()
        {
            dayChangedBinding = new EventBinding<DayChangedEvent>(HandleDayChangedEvent);
            EventBus<DayChangedEvent>.Subscribe(dayChangedBinding);
        }


        private void OnDisable()
        {
            EventBus<DayChangedEvent>.Unsubscribe(dayChangedBinding);
        }

        private void Start()
        {
            if (TimeManager.Instance != null)
            {
                timeService = TimeManager.Instance.TimeService;
            }

            dayText.text = $"Day: {timeService.CurrentTime.Day}";
            timeText.text = timeService.CurrentTime.ToString("HH:mm");
        }

        private void Update()
        {
            if (timeService == null) return;
            UpdateUI();
        }

        private void HandleDayChangedEvent(DayChangedEvent evt)
        {
            dayText.text = evt.CurrentDay.ToString();
        }

        private void UpdateUI()
        {
            int currentMinute = timeService.CurrentTime.Minute;
            if (currentMinute == lastMinute) return;

            lastMinute = currentMinute;

            timeText.text = timeService.CurrentTime.ToString("HH:mm");
        }
    }
}