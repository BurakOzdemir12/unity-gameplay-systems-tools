using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.Observer;
using _Project.Systems.EnvironmentSystem.Time.Enums;
using _Project.Systems.EnvironmentSystem.Time.Events;
using _Project.Systems.EnvironmentSystem.Time.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Time
{
    public class TimeService
    {
        private readonly TimeConfigSo timeData;

        private DateTime currentTime;
        public DateTime CurrentTime => currentTime;

        readonly TimeSpan sunriseTime;
        readonly TimeSpan sunsetTime;

        private bool isDayTime;
        private int currentHour;

        //Cached Variables
        private DivisionsOfDay lastDivision;
        private int lastHour;
        private int lastDay;

        // private readonly Observer<bool> isDayTime;
        // private readonly Observer<int> currentHour;
        // private readonly Observer<int> lastDay;

        public TimeService(TimeConfigSo timeData)
        {
            this.timeData = timeData;

            currentTime = new DateTime(1, 1, 1) + TimeSpan.FromHours(timeData.startHour);

            sunriseTime = TimeSpan.FromHours(timeData.sunriseHour);
            sunsetTime = TimeSpan.FromHours(timeData.sunsetHour);

            lastDay = currentTime.Day;
            lastHour = currentTime.Hour;

            lastDivision = CalculateCurrentDivision();

            // isDayTime = new Observer<bool>(IsDayTime());
            // currentHour = new Observer<int>(currentTime.Hour);

            // DivisionsOfDay currentDivision = CalculateCurrentDivision();
            // bool hasDivisionChanged = currentDivision != lastDivision;

            //Hour Changed Events With Observer But it doesnt necessary

            // currentHour.AddListener((hour) =>
            // {
            //     var evt = new TimeChangedEvent(currentTime, currentDivision,
            //         hasDivisionChanged);
            //     EventBus<TimeChangedEvent>.Publish(evt);
            //
            //     lastDivision = currentDivision;
            // });
            //Day Changed Events
        }


        public void UpdateTime(float deltaTime)
        {
            currentTime = currentTime.AddSeconds(deltaTime * timeData.timeMultiplier);

            //Observers Set
            // isDayTime.Value = IsDayTime();
            // currentHour.Value = currentTime.Hour;

            isDayTime = IsDayTime();
            currentHour = currentTime.Hour;

            if (currentTime.Day != lastDay)
            {
                lastDay = currentTime.Day;
                EventBus<DayChangedEvent>.Publish(new DayChangedEvent(lastDay));
            }


            if (lastHour != currentHour)
            {
                DivisionsOfDay currentDivision = CalculateCurrentDivision();
                bool hasDivisionChanged = currentDivision != lastDivision;

                lastHour = currentHour;
                var evt = new TimeChangedEvent(currentTime, currentDivision,
                    hasDivisionChanged);
                EventBus<TimeChangedEvent>.Publish(evt);

                lastDivision = currentDivision;
            }
        }

        public bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

        private float GetTimeProgress(TimeSpan start, TimeSpan end)
        {
            TimeSpan totalTime = CalculateDifference(start, end);
            TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);
           
            return (float)(elapsedTime.TotalMinutes / totalTime.TotalMinutes);
        }

        public float GetSunRotation()
        {
            if (isDayTime)
            {
                float progress = GetTimeProgress(sunriseTime, sunsetTime);
                return Mathf.Lerp(0, 180, progress);
            }
            else
            {
                float progress = GetTimeProgress(sunsetTime, sunriseTime);
                return Mathf.Lerp(180, 360, progress);
            }
        }

        public float GetMoonRotation()
        {
            if (!isDayTime)
            {
                float progress = GetTimeProgress(sunsetTime, sunriseTime);
                Debug.Log(
                    $"Gece Modu - Saat: {currentTime.TimeOfDay}, Progress: {progress}, Açı: {Mathf.Lerp(0, 180, progress)}");
                return Mathf.Lerp(0, 180, progress);
            }
            else
            {
                float progress = GetTimeProgress(sunriseTime, sunsetTime);
                Debug.Log($"Gündüz Modu (HATA?) - Saat: {currentTime.TimeOfDay}, Progress: {progress}");
                return Mathf.Lerp(180, 360, progress);
            }
        }

        TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
        {
            TimeSpan difference = to - from;

            return difference.TotalHours < 0 ? difference + TimeSpan.FromDays(1) : difference;
        }

        private DivisionsOfDay CalculateCurrentDivision()
        {
            TimeSpan time = currentTime.TimeOfDay;
            if (time < sunriseTime) return DivisionsOfDay.Night;
            if (time < TimeSpan.FromHours(12)) return DivisionsOfDay.Morning;
            if (time < sunsetTime) return DivisionsOfDay.Afternoon;
            return time < TimeSpan.FromHours(22) ? DivisionsOfDay.Evening : DivisionsOfDay.Night;
        }
    }
}