using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.Feedback;
using UnityEngine;

namespace _Project.Systems._Core.Effects.Vfx
{
    public class EffectManager : MonoBehaviour
    {
        private EventBinding<CharacterInteractionEvent> interactionBinding;
        private EventBinding<CharacterCombatActionEvent> combatBinding;


        private void Awake()
        {
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

            if (!profile.TryGetSurfaceFeedback(evt.Surface, evt.Type, evt.ActionTag, out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position);
        }

        private void OnCombatAction(CharacterCombatActionEvent @evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetActionFeedback(evt.Surface, evt.Type, evt.ActionTag, out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position);
        }

        private void SpawnVfx(GameObject vfx, Vector3 position)
        {
            if (vfx == null) return; 
    
            // var fx = Instantiate(vfx, position, Quaternion.identity);
    
            var vfxPrefab = vfx.GetComponent<ParticleSystem>();
            if (vfxPrefab != null) 
                Instantiate(vfxPrefab, position, Quaternion.identity);
        }
    }
}