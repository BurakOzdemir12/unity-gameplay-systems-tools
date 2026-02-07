using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat.Dodge_Roll;
using _Project.Systems.GatheringSystem.ScriptableObjects;
using _Project.Systems.MovementSystem.ScriptableObjects;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.BaseScriptableObjects.Characters
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
        public GatheringDataSo GatheringDataSet;
        public ClimbTypeDataSo[] ClimbTypeDataSet;
        public AttackDataSo[] AttackTypeDataSet;

        // public AnimationProfileSo AnimationProfileData;
    }
}