using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems._Core.EventBus.Interfaces;
using UnityEngine;

namespace _Project.Systems._Core.Feedback
{
    public class CharacterFeedbackListener : MonoBehaviour
    {
        [SerializeField] private CharacterFeedbackProfile profile;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Transform vfxRoot; // Optional parent for spawned VFX

        private EventBinding<CharacterInteractionEvent> interactionBinding;

        private void OnEnable()
        {
            interactionBinding = new EventBinding<CharacterInteractionEvent>(OnInteraction);
            EventBus<CharacterInteractionEvent>.Subscribe(interactionBinding);
        }

        private void OnDisable()
        {
            EventBus<CharacterInteractionEvent>.Unsubscribe(interactionBinding);
        }

        private void OnInteraction(CharacterInteractionEvent evt)
        {
            // Filter: Respond only to events from this GameObject
            if (evt.Source != this.gameObject) return;
            if (profile == null) return;

            AudioClip clipToPlay = null;
            GameObject vfxToSpawn = null;
            float volume = 1f;
            bool found = false;

            // Decision Logic: 
            // 1. Is it a surface-dependent action? (Footstep, Land) -> Check Surface Feedback
            // 2. Is it a generic action? (Attack, GetHit) -> Check Action Feedback
            
            // Note: We can expand this logic. For now, we assume:
            // Footstep/Land use Surface Lookups primarily.
            // Attack/Other use Action Lookups primarily.
            
            // Check Action Feedback first (it might override surface, or be the primary source)
            if (profile.TryGetActionFeedback(evt.Type, evt.ActionTag, out var actionClip, out var actionVfx, out var actionVol))
            {
                clipToPlay = actionClip;
                vfxToSpawn = actionVfx;
                volume = actionVol;
                found = true;
            }
            // If not found in Actions, or if it's explicitly a surface interaction type, look in Surface
            // (You might want Footsteps to ALWAYS look at surface, even if not in Action list)
            else if (evt.Type == Enums.InteractionType.Footstep || evt.Type == Enums.InteractionType.Land)
            {
                if (profile.TryGetSurfaceFeedback(evt.Surface, out var surfaceClip, out var surfaceVfx, out var surfaceVol))
                {
                    clipToPlay = surfaceClip;
                    vfxToSpawn = surfaceVfx;
                    volume = surfaceVol;
                    found = true;
                }
            }

            if (found)
            {
                PlayFeedback(clipToPlay, vfxToSpawn, volume, evt.Position);
            }
        }

        private void PlayFeedback(AudioClip clip, GameObject vfx, float volume, Vector3 position)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }

            if (vfx != null)
            {
                // Instantiate VFX at the contact position (feet)
                Instantiate(vfx, position, Quaternion.identity, vfxRoot);
            }
        }
    }
}
