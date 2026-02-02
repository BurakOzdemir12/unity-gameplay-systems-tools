using System;
using _Project.Systems._Core.EventBus.Interfaces;
using _Project.Systems.EnvironmentSystem.Time.Enums;

namespace _Project.Systems.EnvironmentSystem.Time.Events
{
    public struct TimeChangedEvent : IEvent
    {
        public DateTime TimeOfDay;
        public DivisionsOfDay Division;
        public bool IsDivisionJustChanged;

        public TimeChangedEvent(DateTime timeOfDay, DivisionsOfDay division, bool ısDivisionJustChanged)
        {
            TimeOfDay = timeOfDay;
            Division = division;
            IsDivisionJustChanged = ısDivisionJustChanged;
        }
    }
}