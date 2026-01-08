using System;

namespace _Project.Systems._Core.EventBus.Interfaces
{
    public interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
}