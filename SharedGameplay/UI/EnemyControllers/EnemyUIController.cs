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

        [Header("UI Show Range value")] [SerializeField]
        private float showRange = 20f;

        private float showRangeSqr;

        private Coroutine deathRoutine;
        private WaitForSeconds deathWait = new WaitForSeconds(4f);

        private bool isDead;

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
            showRangeSqr = showRange * showRange;

            if (currentHud == null && EnemyHUDPool.Instance != null)
            {
                InitHUD();
            }
        }

        private void LateUpdate()
        {
            if (!currentHud || !mainCamera) return;

            Vector3 targetPos = offset + headPoint.position;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPos);

            Vector3 distanceToPlayer = transform.position - mainCamera.transform.position;
            float distanceSqr = distanceToPlayer.sqrMagnitude;

            bool isVisible = screenPos.z > 0 && distanceSqr < showRangeSqr && !isDead;

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
                isDead = false;
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
            isDead = true;
            if (currentHud && EnemyHUDPool.Instance)
            {
                currentHud.SetAlertState(false, 0);
                currentHud.SetSuspiciousState(false, 0);

                if (deathRoutine != null) StopCoroutine(deathRoutine);
                deathRoutine = StartCoroutine(DeathRoutine());
            }
        }

        private void HandlePerceptionChange(PerceptionState state, float time)
        {
            if (isDead) return;
            switch (state)
            {
                case PerceptionState.Alerted:
                    currentHud?.SetAlertState(true, time);
                    currentHud?.SetSuspiciousState(false, time);
                    break;
                case PerceptionState.Suspicious:
                    currentHud?.SetAlertState(false, time);
                    currentHud?.SetSuspiciousState(true, time);
                    break;
                case PerceptionState.Calm:
                    currentHud?.SetAlertState(false, time);
                    currentHud?.SetSuspiciousState(false, time);
                    break;
            }
        }

        private IEnumerator DeathRoutine()
        {
            yield return deathWait;
            if (currentHud && EnemyHUDPool.Instance)
            {
                EnemyHUDPool.Instance.ReturnHUD(currentHud);
                currentHud = null;
            }
        }


        private void OnDisable()
        {
            enemyHealth.OnTakeDamage -= HandleTakeDamage;
            enemyHealth.OnDeath -= HandleDeath;
        }
    }
}