using _Project.Systems.SharedGameplay.WeaponLogic.ScriptableObjects;
using UnityEngine;

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
        private WeaponDataSo currentWeaponData;

        public WeaponDataSo CurrentWeaponDataSo => currentWeaponData;


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

        public void SwitchWeapon(WeaponDataSo weaponDataSo)
        {
            WeaponLogic.WeaponLogic newWeaponLogic = Instantiate(weaponDataSo.itemPrefab, currentWeaponRoot.transform)
                .GetComponentInChildren<WeaponLogic.WeaponLogic>();

            currentWeaponData = weaponDataSo;
            currentWeaponLogic = newWeaponLogic;
            currentWeaponHitbox = newWeaponLogic.gameObject;
        }
    }
}