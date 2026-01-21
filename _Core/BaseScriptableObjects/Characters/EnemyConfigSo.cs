using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat.Dodge_Roll;
using _Project.Systems.MovementSystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems._Core.BaseScriptableObjects.Characters
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Characters/Enemy Config")]
    public class EnemyConfigSo:ScriptableObject
    {
        public EnemyMovementDataSo MovementData;
        public EnemyCombatDataSo CombatData;
        public DodgeDataSo DodgeData;
    }
}