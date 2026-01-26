using System;
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

        [Header("Shield Hitbox")] [SerializeField]
        private GameObject currentShieldHitbox;

        public GameObject CurrentShieldHitbox => currentShieldHitbox;

        [Header("Shield Data")] [SerializeField]
        private ShieldDataSo currentShieldData;

        public ShieldDataSo CurrentShieldData => currentShieldData;

        [Header("Shield Logic")] [SerializeField]
        private ShieldLogic currentShieldLogic;

        public ShieldLogic CurrentShieldLogic => currentShieldLogic;

        private void Start()
        {
            ShieldLogic shieldLogic = currentShieldRoot.GetComponentInChildren<ShieldLogic>(true);
            if (shieldLogic == null)
            {
                Debug.LogError($"{name}: ShieldLogic couldn't find in the children!", this);
                return;
            }
            currentShieldLogic = shieldLogic;
            currentShieldHitbox = shieldLogic.gameObject;
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
    }
}