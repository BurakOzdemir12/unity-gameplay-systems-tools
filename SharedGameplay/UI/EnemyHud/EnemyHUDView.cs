using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Systems.SharedGameplay.UI.EnemyHud
{
    public class EnemyHUDView : MonoBehaviour
    {
        [Header("UI Elements")] [Tooltip("Canvas group")] [SerializeField]
        private CanvasGroup canvasGroup;

        [Space(5)] [Header("Health Settings")] [Tooltip("Health Bar Color")] [SerializeField]
        private Color healthBarColor;

        [Tooltip("Health Bar Slider")] [SerializeField]
        private Slider healthSlider;

        [Header("Icon Settings")] [Tooltip("Suspicious Icon")] [SerializeField]
        private Image suspiciousIcon;

        [Tooltip("suspicious Icon Color Gradient ")] [SerializeField]
        private Gradient suspiciousColorGradient;

        [Tooltip("Alert Icon")] [SerializeField]
        private Image alertIcon;

        [Tooltip("Alert Icon Color Gradient ")] [SerializeField]
        private Gradient alertColorGradient;


        private RectTransform rectTransform;

        //Coroutines
        private Coroutine alertCoroutine;
        private Coroutine suspiciousCoroutine;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            alertCoroutine = null;
            suspiciousCoroutine = null;
        }

        public void ResetHUD()
        {
            canvasGroup.alpha = 1f;
            healthSlider.value = 1f;

            StopAllEffects();

            if (alertIcon) alertIcon.gameObject.SetActive(false);
            if (suspiciousIcon) suspiciousIcon.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void SetHealth(float currentHealth, float maxHealth)
        {
            if (maxHealth > 0)
                healthSlider.value = currentHealth / maxHealth;
        }

        public void SetAlertState(bool isAlerted, float duration)
        {
            if (!alertIcon) return;
            if (isAlerted)
            {
                if (alertIcon.gameObject.activeSelf) return;

                alertIcon.gameObject.SetActive(true);
                if (alertCoroutine != null) StopCoroutine(alertCoroutine);
                alertCoroutine = StartCoroutine(AlertedRoutine(duration));
            }
            else
            {
                if (alertCoroutine != null)
                {
                    StopCoroutine(alertCoroutine);
                    alertCoroutine = null;
                }

                alertIcon.gameObject.SetActive(false);
            }
        }

        public void SetSuspiciousState(bool isSuspicious, float duration)
        {
            if (!suspiciousIcon) return;
            if (isSuspicious)
            {
                if (suspiciousIcon.gameObject.activeSelf) return;

                suspiciousIcon.gameObject.SetActive(true);
                if (suspiciousCoroutine != null) StopCoroutine(suspiciousCoroutine);
                suspiciousCoroutine = StartCoroutine(SuspiciousRoutine(duration));
            }
            else
            {
                if (suspiciousCoroutine != null)
                {
                    StopCoroutine(suspiciousCoroutine);
                    suspiciousCoroutine = null;
                }

                suspiciousIcon.gameObject.SetActive(false);
            }
        }

        public void UpdatePosition(Vector3 screenPos, bool isVisible)
        {
            float targetAlpha = isVisible ? 1f : 0f;

            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);

            if (isVisible || canvasGroup.alpha > 0.01f)
            {
                rectTransform.position = screenPos;
            }
        }

        private IEnumerator AlertedRoutine(float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime * 1.5f;
                float value = Mathf.PingPong(timer, 1f);

                alertIcon.color = alertColorGradient.Evaluate(value);

                yield return null;
            }

            if (alertIcon)
            {
                alertIcon.gameObject.SetActive(false);
            }
        }

        //TODO Make one generic function for both routines
        private IEnumerator SuspiciousRoutine(float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime * 1.5f;
                float value = Mathf.PingPong(timer, 1f);

                suspiciousIcon.color = suspiciousColorGradient.Evaluate(value);

                yield return null;
            }

            if (suspiciousIcon)
            {
                suspiciousIcon.gameObject.SetActive(false);
            }
        }

        private void StopAllEffects()
        {
            if (alertCoroutine != null) StopCoroutine(alertCoroutine);
            if (suspiciousCoroutine != null) StopCoroutine(suspiciousCoroutine);

            if (alertIcon) alertIcon.gameObject.SetActive(false);
            if (suspiciousIcon) suspiciousIcon.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            StopAllEffects();
        }
    }
}