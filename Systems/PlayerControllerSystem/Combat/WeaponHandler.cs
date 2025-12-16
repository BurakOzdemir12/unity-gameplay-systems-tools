using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.Combat
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private Transform currentWeaponCollider;

        private void EnableWeapon()
        {
            currentWeaponCollider.gameObject.SetActive(true);
        }

        private void DisableWeapon()
        {
            currentWeaponCollider.gameObject.SetActive(false);
        }
    }
}