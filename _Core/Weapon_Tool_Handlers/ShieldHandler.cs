using System;
using _Project.Systems._Core.Effects.Audio;
using _Project.Systems._Core.Effects.Vfx;
using _Project.Systems._Core.Shield_Logic;
using _Project.Systems._Core.Shield_Logic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.Weapon_Tool_Handlers
{
    public class ShieldHandler : MonoBehaviour
    {
        [Header("Shield Root")] [SerializeField]
        private GameObject currentShieldRoot;

        public GameObject CurrentShieldRoot => currentShieldRoot;

        [Header("Current Shield Model")] [SerializeField]
        private GameObject currentShieldModel;

        public GameObject CurrentShieldModel => currentShieldModel;

        [Header("Shield Hitbox")] [SerializeField]
        private GameObject currentShieldHitbox;

        public GameObject CurrentShieldHitbox => currentShieldHitbox;

        [Header("Shield Data")] [SerializeField]
        private ShieldDataSo currentShieldData;

        public ShieldDataSo CurrentShieldData => currentShieldData;

        [Header("Shield Logic")] [SerializeField]
        private ShieldLogic currentShieldLogic;

        public ShieldLogic CurrentShieldLogic => currentShieldLogic;


        private void Awake()
        {
            if (currentShieldLogic) return;
            ShieldLogic shieldLogic = currentShieldRoot.GetComponentInChildren<ShieldLogic>(true);
            if (shieldLogic == null)
            {
                Debug.LogError($"{name}: ShieldLogic couldn't find in the children!", this);
                return;
            }

            currentShieldLogic = shieldLogic;
            currentShieldHitbox = shieldLogic.gameObject;
            currentShieldModel = shieldLogic.transform.parent.gameObject;
            currentShieldData = shieldLogic.ShieldData;
        }

        private void OnEnable()
        {
            currentShieldLogic.OnShieldBreak += HandleShieldBreak;
        }

        public void EnableShield()
        {
            if (CurrentShieldHitbox != null)
            {
                currentShieldLogic.PerformBlock();
            }
        }

        public void DisableShield()
        {
            if (CurrentShieldHitbox != null)
            {
                currentShieldLogic.EndBlock();
            }
        }

        private void HandleShieldBreak()
        {
            if (!currentShieldLogic.ShieldData.TryGetShieldActionFeedback(
                    out var clip, out var vfx, out var volume
                )) return;

            SoundManager.Instance.PlayShieldBreak(clip, volume);
            EffectManager.Instance.PlayShieldBreak(vfx, CurrentShieldHitbox.transform.position);

            // currentShieldModel.SetActive(false);
            Destroy(currentShieldModel);
        }

        private void OnDisable()
        {
            currentShieldLogic.OnShieldBreak -= HandleShieldBreak;
        }
    }
}