using System;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.Feedback;
using UnityEngine;

namespace _Project.Systems._Core.Effects.Vfx
{
    public class EffectManager : MonoBehaviour
    {
        private EventBinding<CharacterTraversalEvent> interactionBinding;
        private EventBinding<CharacterCombatActionEvent> combatBinding;
        private EventBinding<CharacterGatheringActionEvent> gatheringBinding;
        private EventBinding<WeaponImpactActionEvent> weaponImpactBinding;
        private EventBinding<ToolImpactActionEvent> toolImpactBinding;

        [SerializeField] private Transform vfxParent;

        private void Awake()
        {
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

            toolImpactBinding = new EventBinding<ToolImpactActionEvent>(HandleToolImpact);
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

            if (!profile.TryGetTraversalFeedback(evt.Surface, evt.Type, evt.ActionTag, out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position, Quaternion.identity);
        }

        private void HandleCombatActionEvent(CharacterCombatActionEvent @evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetCombatActionFeedback(evt.Surface, evt.Type, evt.WeaponType, evt.ActionTag,
                    out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position, Quaternion.identity);
        }

        private void HandleWeaponImpactEvent(WeaponImpactActionEvent evt)
        {
            if (evt.SourceTool == null) return;

            var weaponData = evt.WeaponData;
            if (weaponData == null) return;

            var profile = weaponData.weaponImpactFeedbackProfile;
            if (profile == null) return;

            WeaponType impactType = weaponData.weaponType;

            if (!profile.TryGetWeaponImpactActionFeedback(
                    evt.Surface,
                    impactType,
                    evt.Tag,
                    out _,
                    out var vfx,
                    out _)) return;

            SpawnVfx(vfx, evt.Position, Quaternion.LookRotation(evt.Normal));
        }

        private void HandleToolImpact(ToolImpactActionEvent evt)
        {
            if (evt.SourceTool == null) return;

            var toolData = evt.ToolData;
            if (toolData == null) return;

            var profile = toolData.toolImpactFeedbackProfile;
            if (profile == null) return;

            ToolType impactType = toolData.toolType;

            if (!profile.TryGetToolImpactActionFeedback(
                    evt.Surface,
                    impactType,
                    evt.Tag,
                    out _,
                    out var vfx,
                    out _)) return;

            SpawnVfx(vfx, evt.Position, Quaternion.LookRotation(evt.Normal));
        }

        private void HandleGatheringActionEvent(CharacterGatheringActionEvent evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;


            if (!profile.TryGetGatherActionFeedback(evt.Type, evt.ToolType, evt.ActionTag,
                    out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position, Quaternion.identity);
        }


        private void SpawnVfx(GameObject vfx, Vector3 position, Quaternion rotation = default)
        {
            if (vfx == null) return;

            // var fx = Instantiate(vfx, position, Quaternion.identity);

            // var vfxPrefab = vfx.GetComponent<ParticleSystem>();
            // if (vfxPrefab != null)
            Instantiate(vfx, position, rotation, vfxParent);
        }
    }
}