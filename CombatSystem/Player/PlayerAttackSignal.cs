using System;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Player
{
    public sealed class PlayerAttackSignal : MonoBehaviour
    {
        public static event Action<GameObject, GameObject, AttackDataSo> AttackStarted;

        public void RaiseAttack(GameObject explicitTarget, AttackDataSo attackDataSo
        )
        {
            AttackStarted?.Invoke(gameObject, explicitTarget, attackDataSo);
        }
    }
}