using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat.Dodge_Roll;
using _Project.Systems.MovementSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Data.ScriptableObjects.Characters
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/Characters/Player Config")]
    public class PlayerConfigSo : ScriptableObject
    {
        public MovementDataSo MovementData;
        public CombatDataSo CombatData;
        public DodgeDataSo DodgeData;
        public RollDataSo RollData;
        public JumpDataSo JumpData;
        public FallLandDataSo FallLandData;
        public ClimbTypeDataSo[] ClimbTypeDataSet;
        public AttackDataSo[] AttackTypeDataSet;
    }
}