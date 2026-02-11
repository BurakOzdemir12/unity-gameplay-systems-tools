using System.Collections.Generic;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems.CombatSystem.Events;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.Weapon_Tool_Handlers
{
    public class TrailSystem : MonoBehaviour
    {
        private EventBinding<CharacterCombatActionEvent> combatBinding;
        private EventBinding<CharacterGatheringActionEvent> gatheringBinding;

        private const string TRAIL_START = "TrailStart";
        private const string TRAIL_END = "TrailEnd";

        [SerializeField] private WeaponHandler weaponHandler;
        [SerializeField] private ToolHandler toolHandler;

        private readonly Dictionary<Transform, TrailCache> cacheByHitbox = new();

        private struct TrailCache
        {
            public GameObject Instance;
            public TrailRenderer[] Trails;
        }

        private void Awake()
        {
            if (!weaponHandler) weaponHandler = GetComponent<WeaponHandler>();
            if (!toolHandler) toolHandler = GetComponent<ToolHandler>();
        }

        private void OnEnable()
        {
            combatBinding = new EventBinding<CharacterCombatActionEvent>(HandleCombatActionEvent);
            EventBus<CharacterCombatActionEvent>.Subscribe(combatBinding);

            gatheringBinding = new EventBinding<CharacterGatheringActionEvent>(HandleGatheringActionEvent);
            EventBus<CharacterGatheringActionEvent>.Subscribe(gatheringBinding);
        }


        private void OnDisable()
        {
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
            EventBus<CharacterGatheringActionEvent>.Unsubscribe(gatheringBinding);
        }

        private void HandleCombatActionEvent(CharacterCombatActionEvent evt)
        {
            if (evt.Source != gameObject) return;

            if (IsTag(evt.ActionTag, TRAIL_START)) StartTrailForCurrentEquipment();
            else if (IsTag(evt.ActionTag, TRAIL_END)) StopTrailForCurrentEquipment();
        }


        private void HandleGatheringActionEvent(CharacterGatheringActionEvent evt)
        {
            if (evt.Source != gameObject) return;


            if (IsTag(evt.ActionTag, TRAIL_START)) StartTrailForCurrentEquipment();
            else if (IsTag(evt.ActionTag, TRAIL_END)) StopTrailForCurrentEquipment();
        }

        private bool IsTag(string tag, string expected) =>
            string.Equals(tag, expected, System.StringComparison.OrdinalIgnoreCase);


        private void StartTrailForCurrentEquipment()
        {
            if (!TryGetActiveHitboxAndPrefab(out var hitbox, out var trailPrefab)) return;

            var tc = GetOrCreateTrailInstance(hitbox, trailPrefab);

            // For Trail debounce 
            foreach (var t in tc.Trails)
            {
                if (!t) continue;
                t.Clear();
                t.emitting = true;
            }

            cacheByHitbox[hitbox] = tc;
        }

        private void StopTrailForCurrentEquipment()
        {
            if (!TryGetActiveHitboxAndPrefab(out var hitbox, out _)) return;

            if (!cacheByHitbox.TryGetValue(hitbox, out var tc)) return;

            foreach (var t in tc.Trails)
            {
                if (!t) continue;
                t.emitting = false;
            }
        }

        private TrailCache GetOrCreateTrailInstance(Transform hitbox, GameObject prefab)
        {
            if (cacheByHitbox.TryGetValue(hitbox, out var existing) && existing.Instance != null)
                return existing;


            var instance = Instantiate(prefab, hitbox);
            instance.name = prefab.name + "_TrailInstance";

            var trails = instance.GetComponentsInChildren<TrailRenderer>(true);

            foreach (var t in trails)
            {
                if (!t) continue;
                t.emitting = false;
                t.Clear();
            }

            return new TrailCache
            {
                Instance = instance,
                Trails = trails
            };
        }

        private bool TryGetActiveHitboxAndPrefab(out Transform hitbox, out GameObject trailPrefab)
        {
            hitbox = null;
            trailPrefab = null;

            if (weaponHandler != null &&
                weaponHandler.CurrentWeaponHitBox != null &&
                weaponHandler.CurrentWeaponRoot.activeSelf)
            {
                hitbox = weaponHandler.CurrentWeaponHitBox.transform;
                trailPrefab = weaponHandler.CurrentWeaponLogic.WeaponData.trailPrefab;
                return hitbox != null && trailPrefab != null;
            }

            if (toolHandler != null &&
                toolHandler.CurrentToolHitBox != null &&
                toolHandler.CurrentToolRoot.activeSelf)
            {
                hitbox = toolHandler.CurrentToolHitBox.transform;
                trailPrefab = toolHandler.CurrentToolLogic.ToolData.trailPrefab;
                return hitbox != null && trailPrefab != null;
            }

            return false;
        }
    }
}