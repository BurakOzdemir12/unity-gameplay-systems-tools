using _Project.Systems.Core.WeaponLogic;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem
{
    public class WeaponHandler : MonoBehaviour
    {
        [Header("Assign WeaponRoot here (not Hitbox)")] [SerializeField]
        private GameObject currentWeaponRoot;

        private GameObject currentWeaponHitbox;

        private void Start()
        {
            WeaponLogic weaponLogic = currentWeaponRoot.GetComponentInChildren<WeaponLogic>(true);
            if (weaponLogic == null)
            {
                Debug.LogError($"{name}: WeaponRoot couldn't find in the children!", this);
                return;
            }

            currentWeaponHitbox = weaponLogic.gameObject;
            currentWeaponHitbox.SetActive(false);
        }

        private void EnableWeapon()
        {
            if (currentWeaponHitbox != null)
                currentWeaponHitbox.SetActive(true);
        }

        private void DisableWeapon()
        {
            if (currentWeaponHitbox != null)
                currentWeaponHitbox.SetActive(false);
        }
    }
}