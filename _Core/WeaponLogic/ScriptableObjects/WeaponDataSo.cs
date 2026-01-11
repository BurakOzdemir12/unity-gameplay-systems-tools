using _Project.Systems._Core.Enums;
using _Project.Systems._Core.Feedback.Tools_Weapons;
using UnityEngine;

namespace _Project.Systems._Core.WeaponLogic.ScriptableObjects
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon/Weapon Data")]
    public class WeaponDataSo : ScriptableObject
    {
        public ImpactFeedbackProfile impactFeedbackProfile;
        public ImpactActionType ımpactActionType;

        public float damage;
        public float knockback;
        public float staminaCost;
        public float durability;
    }
}