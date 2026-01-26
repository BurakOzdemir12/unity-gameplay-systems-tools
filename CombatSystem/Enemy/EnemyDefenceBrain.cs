using System;
using _Project.Systems._Core.StateMachine.Enemy;
using _Project.Systems.CombatSystem.Player;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Systems.CombatSystem.Enemy
{
    public class EnemyDefenceBrain : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine stateMachine;


        public bool canBlockAttack = false;

        private void Awake()
        {
            if (!stateMachine)
            {
                stateMachine = GetComponent<EnemyStateMachine>();
            }
        }

        private void OnEnable()
        {
            PlayerAttackSignal.AttackStarted += OnPlayerAttackStarted;
        }

        private void OnPlayerAttackStarted(GameObject attacker, GameObject target, AttackDataSo attackData
        )
        {
            if (!attacker) return;
            Collider attackerCollider = attacker.GetComponent<Collider>();
            Transform attackerRoot = attacker.transform.root;
            bool attackerInBuffer = false;

            foreach (var col in stateMachine.BuffersForChase)
            {
                if (!col) continue;

                if (col.transform.root == attackerRoot)
                {
                    attackerInBuffer = true;
                    break;
                }
            }

            if (!attackerInBuffer) return;
            Debug.Log("Attacker in buffer");
            DecideDefenceAction(attackData);
        }

        private void DecideDefenceAction(AttackDataSo attackData)
        {
            float blockScore = stateMachine.EnemyConfigSo.AIBrainData.blockAttackScore;
            float attackScore = attackData.attackScore;

            float blockChance = blockScore / (blockScore * attackScore);
            blockChance = Mathf.Clamp01(blockChance);

            canBlockAttack = Random.value < blockChance;

            Debug.Log($"Block Chance: {blockChance} and Can block: => {canBlockAttack}");
            //TODO Level Factor for the future

            // float levelFactor = enemyLevel / (float)playerLevel;
            // float adjustedDefence = defenceScore * levelFactor;
            // float blockChance = adjustedDefence / (adjustedDefence + attackScore);

            //TODO Difficulty Factor for the future

            // Easy = 0.7
            // Normal = 1.0
            // Hard = 1.3
            //float difficultyMultiplier;

            // float adjustedDefence = defenceScore * difficultyMultiplier;
            // float blockChance = adjustedDefence / (adjustedDefence + attackScore);
        }

        private void OnDisable()
        {
            PlayerAttackSignal.AttackStarted -= OnPlayerAttackStarted;
        }
    }
}