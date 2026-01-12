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
        private EventBinding<WeaponImpactActionEvent> wImpactBinding;

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

            wImpactBinding = new EventBinding<WeaponImpactActionEvent>(HandleImpactEvent);
            EventBus<WeaponImpactActionEvent>.Subscribe(wImpactBinding);
        }


        private void OnDisable()
        {
            EventBus<CharacterTraversalEvent>.Unsubscribe(interactionBinding);
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
            EventBus<WeaponImpactActionEvent>.Unsubscribe(wImpactBinding);
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

            if (!profile.TryGetCombatActionFeedback(evt.Surface, evt.Type, evt.WeaponToolType, evt.ActionTag,
                    out var clip, out var vfx,
                    out var volume)) return;
            SpawnVfx(vfx, evt.Position, Quaternion.identity);
        }

        private void HandleImpactEvent(WeaponImpactActionEvent evt)
        {
            if (evt.SourceTool == null) return;

            var weaponData = evt.WeaponData;
            if (weaponData == null) return;

            var profile = weaponData.impactFeedbackProfile;
            if (profile == null) return;

            WeaponToolType impactType = weaponData.weaponToolType;

            if (!profile.TryGetImpactActionFeedback(
                    evt.Surface,
                    impactType,
                    evt.Tag,
                    out _,
                    out var vfx,
                    out _)) return;

            SpawnVfx(vfx, evt.Position, Quaternion.LookRotation(evt.Normal));
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