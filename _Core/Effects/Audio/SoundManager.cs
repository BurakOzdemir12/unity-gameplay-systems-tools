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
        private EventBinding<CharacterGatheringActionEvent> gatheringBinding;
        private EventBinding<WeaponImpactActionEvent> weaponImpactBinding;
        private EventBinding<ToolImpactActionEvent> toolImpactBinding;
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

            gatheringBinding = new EventBinding<CharacterGatheringActionEvent>(HandleGatheringActionEvent);
            EventBus<CharacterGatheringActionEvent>.Subscribe(gatheringBinding);

            weaponImpactBinding = new EventBinding<WeaponImpactActionEvent>(HandleWeaponImpactEvent);
            EventBus<WeaponImpactActionEvent>.Subscribe(weaponImpactBinding);

            toolImpactBinding = new EventBinding<ToolImpactActionEvent>(HandleToolImpactEvent);
            EventBus<ToolImpactActionEvent>.Subscribe(toolImpactBinding);
        }


        private void OnDisable()
        {
            EventBus<CharacterTraversalEvent>.Unsubscribe(interactionBinding);
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
            EventBus<CharacterGatheringActionEvent>.Unsubscribe(gatheringBinding);
            EventBus<WeaponImpactActionEvent>.Unsubscribe(weaponImpactBinding);
            EventBus<ToolImpactActionEvent>.Unsubscribe(toolImpactBinding);
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
                    evt.WeaponType,
                    evt.ActionTag,
                    out var clip,
                    out _,
                    out var volume))
                return;

            audioSource.PlayOneShot(clip, volume);
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }

        //TODO Create HandleLootActionEvent for impacts and Un/Subscribe
        private void HandleWeaponImpactEvent(WeaponImpactActionEvent evt)
        {
            var weaponData = evt.WeaponData;
            if (weaponData == null) return;

            var profile = weaponData.weaponImpactFeedbackProfile;
            if (profile == null) return;

            WeaponType currentWeaponType = weaponData.weaponType;

            if (!profile.TryGetWeaponImpactActionFeedback(
                    evt.Surface,
                    currentWeaponType,
                    evt.Tag,
                    out var clip,
                    out _,
                    out var volume
                )) return;

            audioSource.PlayOneShot(clip, volume);

            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }

        private void HandleToolImpactEvent(ToolImpactActionEvent evt)
        {
            var toolData = evt.ToolData;
            if (toolData == null) return;

            var profile = toolData.toolImpactFeedbackProfile;
            if (profile == null) return;

            ToolType currentToolType = toolData.toolType;

            if (!profile.TryGetToolImpactActionFeedback(
                    evt.Surface,
                    currentToolType,
                    evt.Tag,
                    out var clip, out _, out var volume
                )) return;

            audioSource.PlayOneShot(clip, volume);
        }

        private void HandleGatheringActionEvent(CharacterGatheringActionEvent evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;

            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetGatherActionFeedback(
                    evt.Type,
                    evt.ToolType, evt.ActionTag,
                    out var clip, out _, out var volume
                )) return;
            audioSource.PlayOneShot(clip, volume);
        }
    }
}