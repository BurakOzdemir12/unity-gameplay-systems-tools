using System;
using _Project.Systems._Core.Enums;
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
        private EventBinding<WeaponImpactActionEvent> impactBinding;
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            interactionBinding = new EventBinding<CharacterTraversalEvent>(HandleTraversalEvent);
            EventBus<CharacterTraversalEvent>.Subscribe(interactionBinding);

            combatBinding = new EventBinding<CharacterCombatActionEvent>(HandleCombatActionEvent);
            EventBus<CharacterCombatActionEvent>.Subscribe(combatBinding);

            impactBinding = new EventBinding<WeaponImpactActionEvent>(HandleImpactEvent);
            EventBus<WeaponImpactActionEvent>.Subscribe(impactBinding);
        }


        private void OnDisable()
        {
            EventBus<CharacterTraversalEvent>.Unsubscribe(interactionBinding);
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
            EventBus<WeaponImpactActionEvent>.Unsubscribe(impactBinding);
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
                    evt.WeaponToolType,
                    evt.ActionTag,
                    out var clip,
                    out _,
                    out var volume))
                return;

            audioSource.PlayOneShot(clip, volume);
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }

        //TODO Create HandleLootActionEvent for impacts and Un/Subscribe
        private void HandleImpactEvent(WeaponImpactActionEvent evt)
        {
            var weaponData = evt.WeaponData;
            if (weaponData == null) return;

            var profile = weaponData.impactFeedbackProfile;
            if (profile == null) return;

            WeaponToolType currentWeaponToolType = weaponData.weaponToolType;

            if (!profile.TryGetImpactActionFeedback(
                    evt.Surface,
                    currentWeaponToolType,
                    evt.Tag,
                    out var clip,
                    out _,
                    out var volume
                )) return;
            
            audioSource.PlayOneShot(clip, volume);
            
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);

        }
    }
}