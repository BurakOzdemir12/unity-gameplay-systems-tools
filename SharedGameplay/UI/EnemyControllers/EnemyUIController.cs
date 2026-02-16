using System;
using System.Collections;
using _Project.Systems.HealthSystem.Health;
using _Project.Systems.HealthSystem.Structs;
using _Project.Systems.PerceptionSystem;
using _Project.Systems.PerceptionSystem.Enums;
using _Project.Systems.SharedGameplay.UI.EnemyHud;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.UI.EnemyControllers
{
    public class EnemyUIController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private EnemyHealth enemyHealth;

        [SerializeField] private EnemyPerceptionController perception;

        [Header("Health and Alert Images Positioning")] [SerializeField]
        private Transform headPoint;

        [SerializeField] private Vector3 offset;

        [Header("UI Elements")] private EnemyHUDView currentHud;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (EnemyHUDPool.Instance != null)
            {
                InitHUD();
            }

            enemyHealth.OnTakeDamage += HandleTakeDamage;
            enemyHealth.OnDeath += HandleDeath;
            perception.OnPerceptionChanged += HandlePerceptionChange;
        }

        private void Start()
        {
            if (currentHud == null && EnemyHUDPool.Instance != null)
            {
                InitHUD();
            }
        }

        private void LateUpdate()
        {
            if (!currentHud || !mainCamera) return;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(headPoint.TransformPoint(offset));
            bool isVisible = screenPos.z > 0;

            currentHud.UpdatePosition(screenPos, isVisible);
        }

        private void InitHUD()
        {
            if (currentHud != null) return;
            if (EnemyHUDPool.Instance == null) return;

            currentHud = EnemyHUDPool.Instance.GetHUD();
            if (currentHud != null)
            {
                currentHud.ResetHUD();
            }
        }

        private void HandleTakeDamage(DamageInfo evt)
        {
            if (currentHud == null)
            {
                Debug.LogError("Current HUD is NULL!");
                return;
            }

            currentHud.SetHealth(enemyHealth.CurrentHealth, enemyHealth.MaxHealth);
        }

        private void HandleDeath()
        {
            if (currentHud && EnemyHUDPool.Instance)
            {
                EnemyHUDPool.Instance.ReturnHUD(currentHud);
                currentHud = null;
            }
        }

        private void HandlePerceptionChange(PerceptionState state)
        {
            switch (state)
            {
                case PerceptionState.Alerted:
                    currentHud?.SetAlertState(true);
                    currentHud?.SetSuspiciousState(false);
                    break;
                case PerceptionState.Suspicious:
                    currentHud?.SetAlertState(false);
                    currentHud?.SetSuspiciousState(true);
                    break;
                case PerceptionState.Calm:
                    currentHud?.SetAlertState(false);
                    currentHud?.SetSuspiciousState(false);
                    break;
            }
        }


        private void OnDisable()
        {
            enemyHealth.OnTakeDamage -= HandleTakeDamage;
            enemyHealth.OnDeath -= HandleDeath;
        }
    }
}