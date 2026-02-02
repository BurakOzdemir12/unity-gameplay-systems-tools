using _Project.Systems._Core.EventBus.Interfaces;

namespace _Project.Systems.EnvironmentSystem.Time.Events
{
    public struct DayChangedEvent : IEvent
    {
        public int CurrentDay;
        // public bool IsDayJustChanged;

        public DayChangedEvent(int currentDay) //, bool ısDayJustChanged
        {
            CurrentDay = currentDay;
            // IsDayJustChanged = ısDayJustChanged;
        }
    }
}