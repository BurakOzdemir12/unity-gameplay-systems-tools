using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.Feedback;
using UnityEngine;

namespace _Project.Systems._Core.Effects.Audio
{
    public class SoundManager : MonoBehaviour
    {
        private EventBinding<CharacterTraversalEvent> interactionBinding;
        private EventBinding<CharacterCombatActionEvent> combatBinding;
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        //TODO Create HandleImpactActionEvent for impacts and Un/Subscribe 

        private void OnEnable()
        {
            interactionBinding = new EventBinding<CharacterTraversalEvent>(HandleTraversalEvent);
            EventBus<CharacterTraversalEvent>.Subscribe(interactionBinding);

            combatBinding = new EventBinding<CharacterCombatActionEvent>(HandleCombatActionEvent);
            EventBus<CharacterCombatActionEvent>.Subscribe(combatBinding);
        }

        private void OnDisable()
        {
            EventBus<CharacterTraversalEvent>.Unsubscribe(interactionBinding);
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
        }

        private void HandleTraversalEvent(CharacterTraversalEvent @evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetTraversalFeedback(
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

        private void HandleCombatActionEvent(CharacterCombatActionEvent evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetCombatActionFeedback(
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