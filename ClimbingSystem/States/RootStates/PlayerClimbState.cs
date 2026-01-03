using _Project.Systems.ClimbingSystem.Enums;
using _Project.Systems.ClimbingSystem.ObstacleScripts;
using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.ClimbingSystem.Structs;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.States.RootStates
{
    public class PlayerClimbState : PlayerBaseState
    {
        private Quaternion targetRotation;
        private ClimbTypeDataSo selectedSo;
        private ParkourDecision selectedDecision;

        public PlayerClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            var hit = stateMachine.ClimbController.CurrentLedgeHitData;
            if (!hit.IsValidLedge)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
            }

            float height = hit.HeightRayHitInfo.point.y
                           - stateMachine.transform.position.y;

            ParkourActionType desiredType = ParkourActionType.Climb;
            var col = hit.ForwardRayHitInfo.collider;
            if (col is null)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
            }

            if (col.TryGetComponent(out ParkourObstacle obstacle))
                desiredType = obstacle.ActionType;
            else
                desiredType = col.GetComponentInParent<ParkourObstacle>()?.ActionType ?? ParkourActionType.Climb;

            // if (hit.forwardRayHitInfo.collider.CompareTag("Vault")) // Simple Way but not the best
            // {
            // assign in inspector
            // var vaultSo = stateMachine.VaultSo; 
            // SetSubState(new PlayerParkourActionState(stateMachine, vaultSo));
            // return;
            // }

            selectedSo = null;
            selectedDecision = ParkourDecision.Invalid;

            Vector3 playerPos = stateMachine.transform.position;
            Vector3 playerRight = stateMachine.transform.right;

            foreach (var so in stateMachine.ClimbTypeDataSet)
            {
                if (so is null) continue;
                if (!so.MatchesActionType(desiredType)) continue;
                var decision = so.Evaluate(height, hit, playerPos, playerRight);
                if (!decision.IsValid) continue;

                // if (so.CheckLedgeHeight(height, hit))
                // {
                selectedSo = so;
                selectedDecision = decision;
                break;
                // }
            }

            if (selectedSo is null)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
            }

            SetSubState(new PlayerParkourActionState(stateMachine, selectedSo, selectedDecision));
            //if You'r going to use different states for each different climb type than use Actionype in So
        }

        public override void Tick(float deltaTime)
        {
        }


        public override void Exit()
        {
            ClearSubState();
        }


        public void SwitchSubState(PlayerBaseState newSubState) => SetSubState(newSubState);
    }
}