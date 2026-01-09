using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.Feedback;
using UnityEngine;

namespace _Project.Systems._Core.Effects.Audio
{
    public class SoundManager : MonoBehaviour
    {
        private EventBinding<CharacterInteractionEvent> interactionBinding;
        private EventBinding<CharacterCombatActionEvent> combatBinding;
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            interactionBinding = new EventBinding<CharacterInteractionEvent>(OnInteraction);
            EventBus<CharacterInteractionEvent>.Subscribe(interactionBinding);

            combatBinding = new EventBinding<CharacterCombatActionEvent>(OnCombatAction);
            EventBus<CharacterCombatActionEvent>.Subscribe(combatBinding);
        }

        private void OnDisable()
        {
            EventBus<CharacterInteractionEvent>.Unsubscribe(interactionBinding);
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
        }

        private void OnInteraction(CharacterInteractionEvent @evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetSurfaceFeedback(
                    evt.Surface,
                    evt.Type,
                    evt.ActionTag,
                    out var clip,
                    out _,
                    out var volume))
                return;

            audioSource.PlayOneShot(clip, volume);
            
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }

        private void OnCombatAction(CharacterCombatActionEvent evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetActionFeedback(
                    evt.Surface,
                    evt.Type,
                    evt.ActionTag,
                    out var clip,
                    out _,
                    out var volume))
                return;

            audioSource.PlayOneShot(clip, volume);
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }
    }
}