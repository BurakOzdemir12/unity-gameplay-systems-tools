using System;
using _Project.Systems.SharedGameplay.WeaponLogic.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Systems.SharedGameplay.Weapon_Tool_Handlers
{
    public class WeaponHandler : MonoBehaviour
    {
        [Header("Assign WeaponRoot here (not Hitbox)")] [SerializeField]
        private GameObject currentWeaponRoot;

        public GameObject CurrentWeaponRoot => currentWeaponRoot;

        [Header("Current Weapon Hitbox ")] [SerializeField]
        private GameObject currentWeaponHitbox;

        public GameObject CurrentWeaponHitBox => currentWeaponHitbox;

        [Header("Current Weapon Logic ")] [SerializeField]
        private WeaponLogic.WeaponLogic currentWeaponLogic;

        public WeaponLogic.WeaponLogic CurrentWeaponLogic => currentWeaponLogic;

        [Header("Current Weapon Model")] [SerializeField]
        private GameObject currentWeaponModel;

        public GameObject CurrentWeaponModel => currentWeaponModel;

        [Header("Current Weapon Data ")] [SerializeField]
        private WeaponData currentWeaponData;

        public WeaponData CurrentWeaponData => currentWeaponData;

        //Test
        [Header("Rock Prefab For Testing Purposes")] [SerializeField]
        private GameObject rockPrefab;

        [SerializeField] private float throwPower = 10f;

        private void Awake()
        {
            WeaponLogic.WeaponLogic weaponLogic =
                currentWeaponRoot.GetComponentInChildren<WeaponLogic.WeaponLogic>(true);
            if (weaponLogic == null)
            {
                Debug.LogError($"{name}: WeaponLogic couldn't find in the children!", this);
                return;
            }

            currentWeaponLogic = weaponLogic;
            currentWeaponHitbox = weaponLogic.gameObject;
            currentWeaponModel = weaponLogic.transform.parent.gameObject;
            currentWeaponData = weaponLogic.WeaponData;
        }

        private void Update()
        {
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                TestRockThrow();
            }
        }

        private void EnableWeapon()
        {
            if (currentWeaponHitbox != null)
                currentWeaponLogic.PerformAttack();
            // currentWeaponHitbox.SetActive(true);
        }

        private void DisableWeapon()
        {
            if (currentWeaponHitbox != null)
                currentWeaponLogic.EndAttack();
            // currentWeaponHitbox.SetActive(false);
        }

        public void SwitchWeapon(WeaponData weaponData)
        {
            WeaponLogic.WeaponLogic newWeaponLogic = Instantiate(weaponData.itemPrefab, currentWeaponRoot.transform)
                .GetComponentInChildren<WeaponLogic.WeaponLogic>();

            currentWeaponData = weaponData;
            currentWeaponLogic = newWeaponLogic;
            currentWeaponHitbox = newWeaponLogic.gameObject;
        }

        private void TestRockThrow()
        {
            if (rockPrefab)
            {
                GameObject rockToThrow =
                    Instantiate(rockPrefab, CurrentWeaponRoot.transform.position + Vector3.up * 1.5f,
                        Quaternion.identity);
                rockToThrow.GetComponent<Rigidbody>().AddForce(transform.forward * throwPower);
            }
        }
    }
}