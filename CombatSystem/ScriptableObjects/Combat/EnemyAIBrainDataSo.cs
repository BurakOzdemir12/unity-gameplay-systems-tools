using UnityEngine;

namespace _Project.Systems.CombatSystem.ScriptableObjects.Combat
{
    [CreateAssetMenu (fileName = "AIBrainData", menuName ="Scriptable Objects/AI/AI Brain Data" )]
    public class EnemyAIBrainDataSo : ScriptableObject
    {
        public float blockAttackScore;
        public float parryAttackScore;
        public float rollAttackScore;
        public float dodgeAttackScore;
    }
}