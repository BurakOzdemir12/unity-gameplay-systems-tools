using System.Collections.Generic;
using System.Linq;
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
            public SurfaceType Surface;
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [System.Serializable]
        public struct ActionFeedbackEntry
        {
            public InteractionType Action;
            public string SpecificTag; // If set, only matches if tag matches. If empty, matches any of that Action type unless a specific one exists? No, keep simple.
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [Header("Surface Feedback")]
        [SerializeField] private List<SurfaceFeedbackEntry> surfaceFeedbackList;

        [Header("Action Feedback")]
        [SerializeField] private List<ActionFeedbackEntry> actionFeedbackList;

        [Header("Defaults")]
        [SerializeField] private AudioClip[] defaultClips;

        public bool TryGetSurfaceFeedback(SurfaceType surface, out AudioClip clip, out GameObject vfx, out float volume)
        {
            clip = null;
            vfx = null;
            volume = 1f;

            foreach (var entry in surfaceFeedbackList)
            {
                if (entry.Surface == surface)
                {
                    if (entry.Clips != null && entry.Clips.Length > 0)
                        clip = entry.Clips[Random.Range(0, entry.Clips.Length)];
                    
                    vfx = entry.VFX;
                    volume = entry.Volume <= 0 ? 1f : entry.Volume;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetActionFeedback(InteractionType action, string tag, out AudioClip clip, out GameObject vfx, out float volume)
        {
            clip = null;
            vfx = null;
            volume = 1f;

            // Priority: Match Action AND Tag -> Match Action only (if tag is empty in entry)
            // If tag is provided in event, we look for entry with that tag.
            // If entry has empty tag, it matches any event of that Action type (fallback).
            
            ActionFeedbackEntry? bestMatch = null;

            foreach (var entry in actionFeedbackList)
            {
                if (entry.Action != action) continue;

                // Exact match (both have tag or both don't)
                if (string.Equals(entry.SpecificTag, tag, System.StringComparison.OrdinalIgnoreCase))
                {
                    bestMatch = entry;
                    break; 
                }
                
                // Generic fallback? (Entry has no tag, but Event has tag OR Event has no tag)
                // If Event has tag "Claw", and we have entry with "Claw", we matched above.
                // If Event has tag "Claw", and we have entry with "", we might preserve as candidate.
                if (string.IsNullOrEmpty(entry.SpecificTag) && bestMatch == null)
                {
                    bestMatch = entry;
                }
            }

            if (bestMatch.HasValue)
            {
                var entry = bestMatch.Value;
                if (entry.Clips != null && entry.Clips.Length > 0)
                    clip = entry.Clips[Random.Range(0, entry.Clips.Length)];
                
                vfx = entry.VFX;
                volume = entry.Volume <= 0 ? 1f : entry.Volume;
                return true;
            }

            return false;
        }
    }
}
