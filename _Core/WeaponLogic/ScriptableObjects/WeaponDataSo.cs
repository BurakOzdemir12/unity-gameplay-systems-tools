using _Project.Systems._Core.Enums;
using _Project.Systems._Core.Feedback.Tools_Weapons;
using _Project.Systems.InventorySystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.WeaponLogic.ScriptableObjects
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon/Weapon Data")]
    public class WeaponDataSo : ItemDataSo
    {
        public WeaponImpactFeedbackProfile weaponImpactFeedbackProfile;
        public WeaponType weaponType;

        public GameObject trailPrefab;
        public float damage;
        public float knockback;
        public float staminaCost;
        public float durability;
    }
}