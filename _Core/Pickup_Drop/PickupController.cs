using System;
using _Project.Systems._Core.Pickup_Drop.Interfaces;
using _Project.Systems._Core.Weapon_Tool_Handlers;
using _Project.Systems._Core.WeaponLogic.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.Pickup_Drop
{
    public class PickupController : MonoBehaviour
    {
        [SerializeField] private WeaponHandler weaponHandler;

        private ScriptableObject itemData;

        private void OnTriggerEnter(Collider other)
        {
            itemData = weaponHandler.CurrentWeaponDataSo;
            if (other.TryGetComponent<IPickupable>(out var pickable) && pickable != null)
            {
                // pickable.PickUp(itemData);
                // weaponHandler.SwitchWeapon((WeaponDataSo)itemData);
            }
        }
    }
}