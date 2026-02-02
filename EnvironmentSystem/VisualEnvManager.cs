using _Project.Systems.EnvironmentSystem.Time;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Project.Systems.EnvironmentSystem
{
    public class VisualEnvManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Lights")] [SerializeField] private Light sun;
        [SerializeField] private Light moon;

        [Header("Light Settings")] [SerializeField]
        private AnimationCurve lightIntensityCurve;

        [SerializeField] private float maxSunIntensity;
        [SerializeField] private float maxMoonIntensity;

        [SerializeField] private Color dayAmbientLight;
        [SerializeField] private Color nightAmbientLight;
        [SerializeField] private Volume volume;
        [SerializeField] private ColorAdjustments colorAdjustments;

        private TimeService timeService;

        private void Start()
        {
            // _timeService = ServiceLocator.Get<TimeService>();
            if (TimeManager.Instance != null)
            {
                timeService = TimeManager.Instance.TimeService;
            }

            timeText.text = timeService.CurrentTime.ToString("hh:mm");

            volume.profile.TryGet(out colorAdjustments);
        }

        private void Update()
        {
            RotateSun();
            UpdateLightSetting();
            UpdateUI();
        }

        private void RotateSun()
        {
            float rotation = timeService.CalculateSunAngle();
            sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
        }

        private void UpdateLightSetting()
        {
            float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
            sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
            moon.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(dotProduct));

            if (colorAdjustments == null) return;

            colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight,
                lightIntensityCurve.Evaluate(dotProduct));
        }

        private void UpdateUI()
        {
            timeText.text = timeService.CurrentTime.ToString("hh:mm");
        }
    }
}