using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat
{
    [CreateAssetMenu(fileName = "CombatData", menuName = "Scriptable Objects/Combat/Combat Data")]
    public class CombatDataSo : ScriptableObject
    {
        [Header("Attack settings")] [SerializeField]
        private string attackType;

        public string AttackType => attackType;

        [Tooltip(" Attack damage")] [SerializeField]
        private float attackDamage = 20f;

        public float AttackDamage => attackDamage;

        [Tooltip(" The time it takes to rotate towards movement direction")] [SerializeField]
        private float rotationDampTimeWhileAttack = 0.1f;

        public float RotationDampTimeWhileAttack => rotationDampTimeWhileAttack;

        [Header("Impact Settings")] [Tooltip(" Impact duration")] [SerializeField]
        private float impactDuration = 0.1f;

        public float ImpactDuration => impactDuration;

        [Header("Blocking Settings")] [Tooltip("The Blocking layer Change speed ")] [SerializeField]
        private float blockingLayerChangeSpeed = 0.1f;

        public float BlockingLayerChangeSpeed => blockingLayerChangeSpeed;


        [Tooltip(" The time it takes to rotate towards movement direction when Block")] [SerializeField]
        private float rotationDampTimeWhileBlock = 2f;

        public float RotationDampTimeWhileBlock => rotationDampTimeWhileBlock;
    }
}