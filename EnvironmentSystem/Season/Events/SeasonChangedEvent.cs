using _Project.Systems._Core.EventBus.Interfaces;
using _Project.Systems.EnvironmentSystem.Season.Enums;

namespace _Project.Systems.EnvironmentSystem.Season.Events
{
    public struct SeasonChangedEvent : IEvent
    {
        public SeasonType CurrentSeasonType;

        public SeasonChangedEvent(Enums.SeasonType currentSeasonType)
        {
            CurrentSeasonType = currentSeasonType;
        }
    }
}