using System;
using System.Collections.Generic;
using _Project.Systems._Core.Enums;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.Feedback.Tools_Weapons
{
    [CreateAssetMenu(fileName = "NewToolImpactProfile", menuName = "Scriptable Objects/Impact Feedback/Tool Impact Profile")]
    public class ToolImpactFeedbackProfile : ScriptableObject
    {
        [System.Serializable]
        public struct ImpactActionFeedbackEntry
        {
            public string Name;
            public SurfaceType Surface;
            public ToolType impactActionType;

            [Tooltip("Can use for specific attack or swing types like light/heavy attack, breaking, etc.")]
            public string SpecificTag;

            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [SerializeField] private List<ImpactActionFeedbackEntry> impactActionFeedbackList;

        // ---------------- IMPACT ACTION ----------------

        public bool TryGetToolImpactActionFeedback(SurfaceType surface, ToolType impactActionType, string tag,
            out AudioClip clip, out GameObject vfx, out float volume)
        {
            clip = null;
            vfx = null;
            volume = 1f;

            ImpactActionFeedbackEntry? fallback = null;

            foreach (var entry in impactActionFeedbackList)
            {
                if (entry.Surface != surface) continue;
                if (entry.impactActionType != impactActionType) continue;

                bool isGenericTag = string.IsNullOrEmpty(entry.SpecificTag);

                bool isTagMatch =
                    isGenericTag ||
                    entry.SpecificTag.Equals(tag, StringComparison.OrdinalIgnoreCase);

                if (!isTagMatch) continue;

                if (isGenericTag)
                {
                    fallback = entry;
                    continue;
                }

                return Fill(entry, out clip, out vfx, out volume);
            }

            if (fallback.HasValue)
                return Fill(fallback.Value, out clip, out vfx, out volume);

            return false;
        }

        private bool Fill(ImpactActionFeedbackEntry entry, out AudioClip clip, out GameObject vfx, out float volume)
        {
            clip = (entry.Clips != null && entry.Clips.Length > 0)
                ? entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)]
                : null;

            vfx = entry.VFX;
            volume = entry.Volume <= 0 ? 1f : entry.Volume;
            return true;
        }
    }
}