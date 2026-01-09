
using System;
using System.Collections.Generic;
using _Project.Systems._Core.Enums;
using UnityEngine;

namespace _Project.Systems._Core.Feedback
{
    [CreateAssetMenu(fileName = "NewCharacterProfile", menuName = "Systems/Feedback/Character Profile")]
    public class CharacterFeedbackProfile : ScriptableObject
    {
        [System.Serializable]
        public struct SurfaceFeedbackEntry
        {
            public string Name;
            public SurfaceType Surface;
            public InteractionType Interaction;
            public string SpecificTag; // Walk / Run / HardLand
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [System.Serializable]
        public struct ActionFeedbackEntry
        {
            public string Name;
            public SurfaceType Surface;
            public ActionType Action;
            public string SpecificTag; // Sword / Claw
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [SerializeField] private List<SurfaceFeedbackEntry> surfaceFeedbackList;
        [SerializeField] private List<ActionFeedbackEntry> actionFeedbackList;

        // ---------------- SURFACE ----------------
        public bool TryGetSurfaceFeedback(
            SurfaceType surface,
            InteractionType interaction,
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
                if (entry.Interaction != interaction) continue;

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
        public bool TryGetActionFeedback(
            SurfaceType surface,
            ActionType action,
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
                if (entry.Action != action) continue;


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