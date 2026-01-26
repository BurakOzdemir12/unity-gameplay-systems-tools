using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat
{
    [CreateAssetMenu(fileName = "NewAttackDataSo",
        menuName = "Scriptable Objects/Combat/Attack Data",
        order = 0)]
    public class AttackDataSo : ScriptableObject
    {
        [Header("Animation Settings")]
        [field: SerializeField]
        public string AnimationName { get; private set; }

        [field: SerializeField] public string AttackType { get; private set; }
        [field: SerializeField] public float TransitionDuration { get; private set; }
        [field: SerializeField] public int ComboStateIndex { get; private set; } = -1;
        [field: SerializeField] public float ComboAttackTime { get; private set; }

        [Header("Attack Settings")]
        [Tooltip("Attack Force To hit direction ")]
        [field: SerializeField]
        public float AttackForce { get; private set; }

        [Tooltip("Attack Force apply time ")]
        [field: SerializeField]
        public float ForceTime { get; private set; }

        [Tooltip("Damage Multiplier by attack type heavy light etc. ")]
        [field: SerializeField]
        public float DamageMultiplier { get; private set; }

        [Tooltip("Attack Knock Back force to Enemy - to opposite character. ")]
        [field: SerializeField]
        public float KnockBackForce { get; private set; }

        [Tooltip("Attack Score For Defence Calculation")]
        public float attackScore;

    }
}