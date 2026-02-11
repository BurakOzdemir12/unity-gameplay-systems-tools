using System;
using System.Collections.Generic;
using _Project.Systems._Core.Enums;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.Feedback
{
    [CreateAssetMenu(fileName = "NewCharacterProfile", menuName = "Scriptable Objects/Feedback/Character Profile")]
    public class CharacterFeedbackProfile : ScriptableObject
    {
        [System.Serializable]
        public struct SurfaceFeedbackEntry
        {
            public string Name;
            public SurfaceType Surface;
            public TraversalType traversal;
            public string SpecificTag; // Walk / Run / HardLand
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [System.Serializable]
        public struct CombatActionFeedbackEntry
        {
            public string Name;
            public SurfaceType Surface;
            public CombatActionType combatAction;
            public WeaponType weaponType;
            public string SpecificTag; // Sword / Claw
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [System.Serializable]
        public struct GatherActionFeedbackEntry
        {
            public string Name;
            public SurfaceType Surface;
            public GatherActionType gatherAction;
            public ToolType ToolType;
            public string SpecificTag; // harvest/gatheer
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [SerializeField] private List<SurfaceFeedbackEntry> surfaceFeedbackList;
        [SerializeField] private List<CombatActionFeedbackEntry> actionFeedbackList;
        [SerializeField] private List<GatherActionFeedbackEntry> gatherActionFeedbackList;

        // ---------------- SURFACE ----------------
        //TODO Disable action tag for traversal, its not necessary 
        public bool TryGetTraversalFeedback(
            SurfaceType surface,
            TraversalType traversal,
            string tag,
            out AudioClip clip,
            out GameObject vfx,
            out float volume)
        {
            clip = null;
            vfx = null;
            volume = 1f;

            foreach (var entry in surfaceFeedbackList)
            {
                if (entry.Surface != surface) continue;
                if (entry.traversal != traversal) continue;

                bool tagMatch =
                    string.IsNullOrEmpty(entry.SpecificTag) ||
                    entry.SpecificTag.Equals(tag, StringComparison.OrdinalIgnoreCase);

                if (!tagMatch) continue;

                if (entry.Clips.Length > 0)
                    clip = entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];

                vfx = entry.VFX;
                volume = entry.Volume <= 0 ? 1f : entry.Volume;
                return true;
            }

            return false;
        }

        // ---------------- ACTION ----------------
        public bool TryGetCombatActionFeedback(
            SurfaceType surface,
            CombatActionType combatAction,
            WeaponType weaponType,
            string tag,
            out AudioClip clip,
            out GameObject vfx,
            out float volume)
        {
            clip = null;
            vfx = null;
            volume = 1f;


            foreach (var entry in actionFeedbackList)
            {
                if (entry.Surface != surface) continue;
                if (entry.combatAction != combatAction) continue;
                if (entry.weaponType != weaponType) continue;

                bool isGeneric = string.IsNullOrEmpty(entry.SpecificTag);
                bool tagMatch = isGeneric ||
                                entry.SpecificTag.Equals(tag, StringComparison.OrdinalIgnoreCase);

                if (!tagMatch) continue;

                if (entry.Clips.Length > 0)
                    clip = entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];


                vfx = entry.VFX;
                volume = entry.Volume <= 0 ? 1f : entry.Volume;
                return true;
            }

            return false;
        }
        // ---------------- LOOT ACTION ----------------

        public bool TryGetGatherActionFeedback(
            GatherActionType gatherAction, ToolType toolType, string tag,
            out AudioClip clip, out GameObject vfx, out float volume) //SurfaceType surface,
        {
            clip = null;
            vfx = null;
            volume = 1f;

            foreach (var entry in gatherActionFeedbackList)
            {
                // if (entry.Surface != surface) continue;
                if (entry.gatherAction != gatherAction) continue;
                if (entry.ToolType != toolType) continue;


                bool tagMatch =
                    string.IsNullOrEmpty(entry.SpecificTag) ||
                    entry.SpecificTag.Equals(tag, StringComparison.OrdinalIgnoreCase);

                if (!tagMatch) continue;

                if (entry.Clips.Length > 0)
                    clip = entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];


                vfx = entry.VFX;
                volume = entry.Volume <= 0 ? 1f : entry.Volume;
                return true;
            }

            return false;
        }
    }
}