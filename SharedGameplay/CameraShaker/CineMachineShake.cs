using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.CameraShaker
{
    public class CineMachineShake : MonoBehaviour
    {
        private CinemachineBasicMultiChannelPerlin perlin;
        private EventBinding<WeaponImpactActionEvent> weaponImpactBinding;

        private float shakeTimer;
        private float shakeTimerTotal;
        private float startingIntensity;

        private void Awake()
        {
            perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (perlin == null)
            {
                Debug.LogError("CinemachineBasicMultiChannelPerlin not found! " +
                               "Make sure you have added a Noise Profile in the Inspector.");
            }
        }

        private void OnEnable()
        {
            weaponImpactBinding = new EventBinding<WeaponImpactActionEvent>(HandleWeaponImpact);
            EventBus<WeaponImpactActionEvent>.Subscribe(weaponImpactBinding);
        }

        private void OnDisable()
        {
            EventBus<WeaponImpactActionEvent>.Unsubscribe(weaponImpactBinding);
        }

        private void Update()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;

                // Smoothly reduce the Amplitude (Intensity) to 0 over time
                perlin.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
            else if (perlin != null && perlin.AmplitudeGain > 0)
            {
                // Ensure the shake stops completely when the timer runs out
                perlin.AmplitudeGain = 0f;
            }
        }

        private void HandleWeaponImpact(WeaponImpactActionEvent evt)
        {
            // Only shake if the source of the event is the Player
            if (evt.Source == null || !evt.Source.CompareTag("Player")) return;

            ApplyShake(0.2f, 2f, 12f);
        }

        public void ApplyShake(float duration, float intensity, float frequency)
        {
            if (perlin == null) return;

            perlin.AmplitudeGain = intensity;
            perlin.FrequencyGain = frequency;

            startingIntensity = intensity;
            shakeTimerTotal = duration;
            shakeTimer = duration;
        }

       
    }
}