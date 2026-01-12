using _Project.Systems._Core.WeaponLogic;
using UnityEngine;

namespace _Project.Systems.MovementSystem
{
    public class WeaponHandler : MonoBehaviour
    {
        [Header("Assign WeaponRoot here (not Hitbox)")] [SerializeField]
        private GameObject currentWeaponRoot;

        private GameObject currentWeaponHitbox;
        public GameObject CurrentWeaponHitBox => currentWeaponHitbox;

        private WeaponLogic currentWeaponLogic;
        public WeaponLogic CurrentWeaponLogic => currentWeaponLogic;

        private void Start()
        {
            WeaponLogic weaponLogic = currentWeaponRoot.GetComponentInChildren<WeaponLogic>(true);
            if (weaponLogic == null)
            {
                Debug.LogError($"{name}: WeaponRoot couldn't find in the children!", this);
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