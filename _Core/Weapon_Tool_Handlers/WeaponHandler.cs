using UnityEngine;

namespace _Project.Systems._Core.Weapon_Tool_Handlers
{
    public class WeaponHandler : MonoBehaviour
    {
        [Header("Assign WeaponRoot here (not Hitbox)")] [SerializeField]
        private GameObject currentWeaponRoot;

        [Header("Weapon Logic ")] private GameObject currentWeaponHitbox;
        public GameObject CurrentWeaponHitBox => currentWeaponHitbox;

        private WeaponLogic.WeaponLogic currentWeaponLogic;
        public WeaponLogic.WeaponLogic CurrentWeaponLogic => currentWeaponLogic;


        private void Start()
        {
            WeaponLogic.WeaponLogic weaponLogic = currentWeaponRoot.GetComponentInChildren<WeaponLogic.WeaponLogic>(true);
            if (weaponLogic == null)
            {
                Debug.LogError($"{name}: WeaponLogic couldn't find in the children!", this);
                return;
            }

            currentWeaponLogic = weaponLogic;
            currentWeaponHitbox = weaponLogic.gameObject;
            currentWeaponHitbox.SetActive(false);
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
    }
}