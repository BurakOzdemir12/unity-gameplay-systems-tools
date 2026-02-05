using System;
using System.Collections;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Time;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using _Project.Systems.EnvironmentSystem.Weather.Events;
using _Project.Systems.EnvironmentSystem.Weather.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Project.Systems.EnvironmentSystem
{
    public class SkyboxManager : MonoBehaviour
    {
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


        [Space(2)] [Header("Skybox Settings")] [Tooltip("Material used for Skybox Blending")] [SerializeField]
        private Material skyboxMaterial;

        #region Shader Properties

        // Sun Settings
        private static readonly int SUN_SIZE_ID = Shader.PropertyToID("_SunSize");

        private static readonly int SUN_HAZE_ID = Shader.PropertyToID("_SunHaze");

        // Atmosphere & Colors
        private static readonly int ATMOSPHERE_THICKNESS_ID = Shader.PropertyToID("_AtmosphereThickness");
        private static readonly int ZENITH_COLOR_ID = Shader.PropertyToID("_ZenithColor");
        private static readonly int HORIZON_COLOR_ID = Shader.PropertyToID("_HorizonColor");
        private static readonly int GROUND_COLOR_ID = Shader.PropertyToID("_GroundColor");
        private static readonly int SKY_EXPOSURE_ID = Shader.PropertyToID("_SkyExposure");

        private static readonly int SKY_SATURATION_ID = Shader.PropertyToID("_SkySaturation");

        // Horizon Settings
        private static readonly int HORIZON_HEIGHT_ID = Shader.PropertyToID("_HorizonHeight");

        private static readonly int HORIZON_SHARPNESS_ID = Shader.PropertyToID("_HorizonSharpness");

        // Moon Settings
        private static readonly int ENABLE_MOON_ID = Shader.PropertyToID("_EnableMoon");
        private static readonly int MOON_TEX_ID = Shader.PropertyToID("_MoonTex");

        private static readonly int MOON_SIZE_ID = Shader.PropertyToID("_MoonSize");

        // Stars Settings
        private static readonly int ENABLE_STARS_ID = Shader.PropertyToID("_EnableStars");
        private static readonly int STAR_SEED_ID = Shader.PropertyToID("_StarSeed");
        private static readonly int STAR_DENSITY_ID = Shader.PropertyToID("_StarDensity");

        private static readonly int STAR_INTENSITY_ID = Shader.PropertyToID("_StarIntensity");

        // Cloud Settings
        private static readonly int CLOUD_TINT_ID = Shader.PropertyToID("_CloudTint");
        private static readonly int CLOUD_SEED_ID = Shader.PropertyToID("_CloudSeed");
        private static readonly int CLOUD_ROTATION_Y_ID = Shader.PropertyToID("_CloudRotationY");
        private static readonly int CLOUD_COVERAGE_ID = Shader.PropertyToID("_CloudCoverage");
        private static readonly int CLOUD_SOFTNESS_ID = Shader.PropertyToID("_CloudSoftness");
        private static readonly int CLOUD_SCALE_ID = Shader.PropertyToID("_CloudScale");
        private static readonly int CLOUD_BASE_HEIGHT_ID = Shader.PropertyToID("_CloudBaseHeight");
        private static readonly int CLOUD_WIND_DIRECTION_ID = Shader.PropertyToID("_CloudWindDirection");
        private static readonly int CLOUD_SPEED_ID = Shader.PropertyToID("_CloudSpeed");

        [Header("Day/Night Skybox Settings")] [Tooltip("Sun Size")] [SerializeField] [Range(0, 1)]
        private float sunSize;

        [Tooltip("Sun Haze")] [SerializeField] [Range(0, 1)]
        private float sunHaze;

        [Tooltip("Day Time Atmosphere Thickness")] [SerializeField] [Range(0, 5)]
        private float dayTimeAtmosphereThickness = 0.5f;

        [Tooltip("Night Time Atmosphere Thickness")] [SerializeField] [Range(0, 1000)]
        private float nightTimeAtmosphereThickness = 1;

        [Tooltip("Gradient used for Skybox Zenith Color")] [SerializeField]
        private Gradient skyColorGradient;

        [Tooltip("Gradient used for Skybox Horizon Color")] [SerializeField]
        private Gradient horizonColorGradient;

        [Tooltip("Gradient used for Skybox Ground Color")] [SerializeField]
        private Gradient groundColorGradient;

        [Space(5)] [Tooltip("Sky Exposure Day")] [SerializeField] [Range(0, 4)]
        private float daySkyExposure = 1;

        [Tooltip("Sky Exposure Night")] [SerializeField] [Range(0, 4)]
        private float nightSkyExposure = 0.5f;

        [Tooltip("Sky Saturation Day")] [SerializeField] [Range(0, 2)]
        private float daySkySaturation = 1;

        [Tooltip("Sky Saturation Night")] [SerializeField] [Range(0, 2)]
        private float nightSkySaturation = 0.5f;

        [Space(5)] [Tooltip("Horizon Height Day")] [SerializeField] [Range(-1, 1)]
        private float dayHorizonHeight = 0.5f;

        [Tooltip("Horizon Height Night")] [SerializeField] [Range(-1, 1)]
        private float nightHorizonHeight = 0.25f;

        [Tooltip("Horizon Sharpness Day")] [SerializeField] [Range(0, 1)]
        private float dayHorizonSharpness = 0.5f;

        [Tooltip("Horizon Sharpness Night")] [SerializeField] [Range(0, 1)]
        private float nightHorizonSharpness = 0.25f;

        [Space(5)] [Tooltip("Moon texture")] [SerializeField]
        private Texture moonTexture;

        [Tooltip("Moon Enable")] [SerializeField]
        private bool enableMoon = false;

        [Tooltip("Moon Size Night")] [SerializeField] [Range(0, 1)]
        private float moonSize = 0.25f;

        [Header("Star settings")] [Space(5)] [Tooltip("Star Enable")] [SerializeField]
        private bool enableStars = false;

        [Tooltip("Star Seed")] [SerializeField] [Range(0, 1000)]
        private int starSeed = 1;

        [Tooltip("Star Density")] [SerializeField] [Range(0, 500)]
        private float starDensity = 0.005f;

        [Tooltip("Star Intensity")] [SerializeField] [Range(0, 500)]
        private float starIntensity = 0.5f;

        [Header("Cloud Settings")] [Space(5)] [Tooltip("Cloud Tint Gradient color")] [SerializeField]
        private Gradient cloudTintGradient;

        [Tooltip("Cloud Seed")] [SerializeField] [Range(0, 1000)]
        private int cloudSeed = 1;

        [Tooltip("Cloud Rotation Y")] [SerializeField] [Range(0, 360)]
        private float cloudRotationY = 180;

        [Tooltip("Cloud Coverage")] [SerializeField] [Range(0, 1)]
        private float cloudCoverage = 1f;

        [Tooltip("Cloud Softness")] [SerializeField] [Range(0, 2)]
        private float cloudSoftness = 0.5f;

        [Tooltip("Cloud Scale")] [SerializeField] [Range(0, 2)]
        private float cloudScale = 1f;

        [Tooltip("Cloud Base Height")] [SerializeField] [Range(0, 1)]
        private float cloudBaseHeight = 0.25f;

        [Tooltip("Cloud Wind Direction")] [SerializeField]
        private Vector3 cloudWindDirection = new Vector3(1, 0, 0);

        [Tooltip("Cloud Speed")] [SerializeField] [Range(0, 10)]
        private float cloudSpeed = 0.5f;

        #endregion

        //Time Service
        private TimeService timeService;

        private void Start()
        {
            if (TimeManager.Instance != null)
            {
                timeService = TimeManager.Instance.TimeService;
            }

            volume.profile.TryGet(out colorAdjustments);

            if (moonTexture != null)
            {
                skyboxMaterial.SetTexture(MOON_TEX_ID, moonTexture);
            }

            InitializeStaticSkySettings();
        }

        private void Update()
        {
            RotateSunAndMoon();
            UpdateLightSetting();
            HandleSkyBoxBlend();
        }

        private void RotateSunAndMoon()
        {
            float rotation = timeService.CalculateSunAngle();
            sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
            moon.transform.rotation = Quaternion.AngleAxis(rotation - 180f, Vector3.right);
        }

        private void UpdateLightSetting()
        {
            float sunDot = Vector3.Dot(sun.transform.forward, Vector3.down);
            float moonDot = Vector3.Dot(moon.transform.forward, Vector3.down);
            sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(sunDot));
            moon.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(moonDot));

            if (colorAdjustments == null) return;
            colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight,
                lightIntensityCurve.Evaluate(sunDot));
        }

        private void HandleSkyBoxBlend()
        {
            if (!skyboxMaterial) return;
            TimeSpan currentTime = timeService.CurrentTime.TimeOfDay;
            float timePercent = (float)currentTime.TotalHours / 24;

            // float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
            float dayNightT = Mathf.Clamp01(Mathf.Sin(timePercent * Mathf.PI));

            // Sun Size
            float targetSunSize = Mathf.Lerp(0, sunSize, dayNightT);
            skyboxMaterial.SetFloat(SUN_SIZE_ID, targetSunSize);
            // Sun Haze
            float targetSunHaze = Mathf.Lerp(0, sunHaze, dayNightT);
            skyboxMaterial.SetFloat(SUN_HAZE_ID, targetSunHaze);
            // Atmosphere Thickness
            float targetAtmosphereThickness =
                Mathf.Lerp(nightTimeAtmosphereThickness, dayTimeAtmosphereThickness, dayNightT);
            skyboxMaterial.SetFloat(ATMOSPHERE_THICKNESS_ID, targetAtmosphereThickness);
            // Sky Zenith Color
            Color targetSkyZenithColor = skyColorGradient.Evaluate(dayNightT);
            skyboxMaterial.SetColor(ZENITH_COLOR_ID, targetSkyZenithColor);
            // Sky Horizon Color
            Color targetSkyHorizonColor = horizonColorGradient.Evaluate(dayNightT);
            skyboxMaterial.SetColor(HORIZON_COLOR_ID, targetSkyHorizonColor);
            // Sky Ground Color
            Color targetSkyGroundColor = groundColorGradient.Evaluate(dayNightT);
            skyboxMaterial.SetColor(GROUND_COLOR_ID, targetSkyGroundColor);
            // Sky Exposure
            float targetSkyExposure = Mathf.Lerp(nightSkyExposure, daySkyExposure, dayNightT);
            skyboxMaterial.SetFloat(SKY_EXPOSURE_ID, targetSkyExposure);
            // Sky Saturation
            float targetSkySaturation = Mathf.Lerp(nightSkySaturation, daySkySaturation, dayNightT);
            skyboxMaterial.SetFloat(SKY_SATURATION_ID, targetSkySaturation);
            // Horizon Height
            float targetHorizonHeight = Mathf.Lerp(nightHorizonHeight, dayHorizonHeight, dayNightT);
            skyboxMaterial.SetFloat(HORIZON_HEIGHT_ID, targetHorizonHeight);

            // Horizon Sharpness
            float targetHorizonSharpness = Mathf.Lerp(nightHorizonSharpness, dayHorizonSharpness, dayNightT);
            skyboxMaterial.SetFloat(HORIZON_SHARPNESS_ID, targetHorizonSharpness);
            // Moon Size
            float targetMoonSize = Mathf.Lerp(0, moonSize, dayNightT);
            skyboxMaterial.SetFloat(MOON_SIZE_ID, targetMoonSize);
            // Enable Moon
            enableMoon = !timeService.IsDayTime();
            skyboxMaterial.SetFloat(ENABLE_MOON_ID, enableMoon ? 1 : 0);
            // Cloud Tint
            Color targetCloudTint = cloudTintGradient.Evaluate(dayNightT);
            skyboxMaterial.SetColor(CLOUD_TINT_ID, targetCloudTint);
        }

        private void InitializeStaticSkySettings()
        {
            // Enable Stars
            enableStars = !timeService.IsDayTime();
            skyboxMaterial.SetFloat(ENABLE_STARS_ID, enableStars ? 1 : 0);
            // Star Seed
            skyboxMaterial.SetFloat(STAR_SEED_ID, starSeed);
            // Star Density
            skyboxMaterial.SetFloat(STAR_DENSITY_ID, starDensity);
            // Star Intensity
            skyboxMaterial.SetFloat(STAR_INTENSITY_ID, starIntensity);

            // Cloud Seed
            skyboxMaterial.SetFloat(CLOUD_SEED_ID, cloudSeed);
            //Cloud rotation Y
            skyboxMaterial.SetFloat(CLOUD_ROTATION_Y_ID, cloudRotationY);
            // Cloud Coverage
            skyboxMaterial.SetFloat(CLOUD_COVERAGE_ID, cloudCoverage);
            // Cloud Softness
            skyboxMaterial.SetFloat(CLOUD_SOFTNESS_ID, cloudSoftness);
            // Cloud Scale
            skyboxMaterial.SetFloat(CLOUD_SCALE_ID, cloudScale);
            // Cloud Base Height
            skyboxMaterial.SetFloat(CLOUD_BASE_HEIGHT_ID, cloudBaseHeight);
            // Cloud Wind Direction
            skyboxMaterial.SetVector(CLOUD_WIND_DIRECTION_ID, cloudWindDirection);
            // Cloud Speed
            skyboxMaterial.SetFloat(CLOUD_SPEED_ID, cloudSpeed);
        }
    }
}