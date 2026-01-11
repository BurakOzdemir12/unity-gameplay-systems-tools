using System;
using System.Collections.Generic;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.WeaponLogic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.Feedback.Tools_Weapons
{
    [CreateAssetMenu(fileName = "NewImpactProfile", menuName = "Scriptable Objects/Impact Feedback/Impact Profile")]
    public class ImpactFeedbackProfile : ScriptableObject
    {
        [System.Serializable]
        public struct ImpactActionFeedbackEntry
        {
            public string Name;
            public SurfaceType Surface;
            public ImpactActionType ımpactActionType;
            public string SpecificTag; // Use for specific impact actions heavy/light attack, breaking, etc.
            public AudioClip[] Clips;
            public GameObject VFX;
            public float Volume;
        }

        [SerializeField] private List<ImpactActionFeedbackEntry> impactActionFeedbackList;

        // ---------------- IMPACT ACTION ----------------

        public bool TryGetImpactActionFeedback(SurfaceType surface, ImpactActionType ımpactActionType, string tag,
            out AudioClip clip, out GameObject vfx, out float volume)
        {
            clip = null;
            vfx = null;
            volume = 1f;

            ImpactActionFeedbackEntry? fallback = null;

            foreach (var entry in impactActionFeedbackList)
            {
                if (entry.Surface != surface) continue;
                if (entry.ımpactActionType != ımpactActionType) continue;

                bool isGenericTag = string.IsNullOrEmpty(entry.SpecificTag);

                bool isMatch =
                    isGenericTag ||
                    entry.SpecificTag.Equals(tag, StringComparison.OrdinalIgnoreCase);

                if (!isMatch) continue;

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