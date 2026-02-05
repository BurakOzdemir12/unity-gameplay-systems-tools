using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Season.Events;
using TMPro;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.Season.UI
{
    public class SeasonDisplayUI : MonoBehaviour
    {
        private EventBinding<SeasonChangedEvent> seasonChangedBinding;
        [SerializeField] private TextMeshProUGUI seasonText;

        private void OnEnable()
        {
            seasonChangedBinding = new EventBinding<SeasonChangedEvent>(HandleSeasonChangedEvent);
            EventBus<SeasonChangedEvent>.Subscribe(seasonChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<SeasonChangedEvent>.Unsubscribe(seasonChangedBinding);
        }

        private void HandleSeasonChangedEvent(SeasonChangedEvent evt)
        {
            seasonText.text = evt.CurrentSeasonType.ToString();
        }
    }
}