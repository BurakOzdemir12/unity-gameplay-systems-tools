using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems._Core.Weapon_Tool_Handlers
{
    public class WeaponHandler : MonoBehaviour
    {
        [Header("Assign WeaponRoot here (not Hitbox)")] [SerializeField]
        private GameObject currentWeaponRoot;

        public GameObject CurrentWeaponRoot => currentWeaponRoot;

        [Header("Weapon Logic ")] private GameObject currentWeaponHitbox;
        public GameObject CurrentWeaponHitBox => currentWeaponHitbox;

        private WeaponLogic.WeaponLogic currentWeaponLogic;
        public WeaponLogic.WeaponLogic CurrentWeaponLogic => currentWeaponLogic;

        [SerializeField] private GameObject weaponSheatHolderBack;
        [SerializeField] private GameObject weaponHolderRightHand;

        private GameObject currentWeaponInHand;
        private GameObject currentWeaponInSheath;

        private void Start()
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

        public void DrawWeapon()
        {
            currentWeaponInHand = Instantiate(currentWeaponLogic.WeaponData.weaponPrefab,
                weaponHolderRightHand.transform);
            currentWeaponInHand.transform.localPosition = Vector3.zero;
            Destroy(currentWeaponInSheath);
        }

        public void SheathWeapon()
        {
            currentWeaponInSheath =
                Instantiate(currentWeaponLogic.WeaponData.weaponPrefab,
                    weaponSheatHolderBack.transform);
            currentWeaponInSheath.transform.localPosition = Vector3.zero;

            Destroy(currentWeaponInHand);
        }

    }
}