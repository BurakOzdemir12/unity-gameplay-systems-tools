using System;
using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.CombatSystem.Player;
using _Project.Systems.CombatSystem.ScriptableObjects.Combat;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Systems.CombatSystem.Enemy
{
    public class EnemyDefenceBrain : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine stateMachine;


        public bool canBlockAttack = false;
        public bool canParryAttack = false;

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
            // Collider attackerCollider = attacker.GetComponent<Collider>();
            // Transform attackerRoot = attacker.transform.root;
            // bool attackerInBuffer = false;
            
            // foreach (var col in stateMachine.EnemyPerceptionController.BufferSetForChase)
            // {
            //     if (!col) continue;
            //
            //     if (col.transform.root == attackerRoot)
            //     {
            //         attackerInBuffer = true;
            //         break;
            //     }
            // }
            bool isPerceived = stateMachine.EnemyPerceptionController.IsPerceivingTarget(attacker);
            
            if (!isPerceived) return;
            // if (!attackerInBuffer) return;
            DecideDefenceAction(attackData);
        }

        private void DecideDefenceAction(AttackDataSo attackData)
        {
            if (!stateMachine.ShieldHandler.CurrentShieldLogic)
            {
                canBlockAttack = false;
                canParryAttack = false;
                return;
            }

            float attackScore = attackData.attackScore;
            EnemyAIBrainDataSo brainData = stateMachine.EnemyConfigSo.AIBrainData;

            // Block Calculation
            float blockScore = brainData.blockAttackScore;

            float blockChance = blockScore / (blockScore + attackScore);
            blockChance = Mathf.Clamp01(blockChance);

            canBlockAttack = Random.value < blockChance;

            Debug.Log($"Block Chance: {blockChance} and Can block: => {canBlockAttack}");

            //Parry Calculation
            float parryScore = brainData.parryAttackScore;

            float parryChance = parryScore / (parryScore + attackScore);
            parryChance = Mathf.Clamp01(parryChance);

            canParryAttack = Random.value < parryChance;

            Debug.Log($"Parry Chance: {parryChance} and Can Parry: => {canParryAttack}");

            SetEnemyState();
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

        private void SetEnemyState()
        {
            if (stateMachine.CurrentState is EnemyDeadState or EnemyImpactState or EnemyAttackingState)
                return;

            if (!stateMachine.ShieldHandler.CurrentShieldLogic)
            {
                canParryAttack = false;
                return;
            }

            if (canParryAttack)
            {
                stateMachine.SwitchState(new EnemyParryState(stateMachine));
            }
        }

        private void OnDisable()
        {
            PlayerAttackSignal.AttackStarted -= OnPlayerAttackStarted;
        }
    }
}