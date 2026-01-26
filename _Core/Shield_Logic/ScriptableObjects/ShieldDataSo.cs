using _Project.Systems._Core.Enums;
using _Project.Systems._Core.Feedback.Tools_Weapons;
using UnityEngine;

namespace _Project.Systems._Core.Shield_Logic.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ShieldData", menuName = "Scriptable Objects/Shield/Shield Data")]
    public class ShieldDataSo : ScriptableObject
    {
        public WeaponImpactFeedbackProfile weaponImpactFeedbackProfile;

        public ShieldType shieldType;
        public float shieldKnockbackForce;
        public float shieldDamage;
        public float shieldStunPower;
        public float shieldStaminaCost;
        public float shieldDurability;
    }
}