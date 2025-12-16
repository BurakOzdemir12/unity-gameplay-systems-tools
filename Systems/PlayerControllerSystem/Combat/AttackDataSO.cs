using System;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.Combat
{
    [CreateAssetMenu(fileName = "NewAttackDataSo",
        menuName = "Scriptable Objects/_Project/Player Controller System/Combat/Attack Data",
        order = 0)]
    public class AttackDataSo : ScriptableObject
    {
        [field: SerializeField] public string AnimationName { get; private set; }
        [field: SerializeField] public float TransitionDuration { get; private set; }
        [field: SerializeField] public int ComboStateIndex { get; private set; } = -1;
        [field: SerializeField] public float ComboAttackTime { get; private set; }
        [field: SerializeField] public float AttackForce { get; private set; }
        [field: SerializeField] public float ForceTime { get; private set; }
        [field: SerializeField] public float DamageMultiplier { get; private set; }
    }
}