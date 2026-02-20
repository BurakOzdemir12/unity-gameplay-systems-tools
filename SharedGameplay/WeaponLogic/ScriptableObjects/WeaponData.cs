using _Project.Systems._Core.Enums;
using _Project.Systems.InventorySystem.ScriptableObjects;
using _Project.Systems.SharedGameplay.Feedback.Tools_Weapons;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.WeaponLogic.ScriptableObjects
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon/Weapon Data")]
    public class WeaponData : ItemData
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