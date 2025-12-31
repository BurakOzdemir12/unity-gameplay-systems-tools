using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.States.RootStates
{
    public class PlayerClimbState : PlayerBaseState
    {
        private float detectedHeight;

        // private Vector3 wallNormal;
        private Vector3 surfacePoint;
        private Quaternion targetRotation;

        public PlayerClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            float height = stateMachine.ClimbController.CurrentLedgeHitData.heightRayHitInfo.point.y
                           - stateMachine.transform.position.y;

            ClimbTypeDataSo selectedSo = null;

            foreach (var so in stateMachine.ClimbTypeDataSet)
            {
                if (so != null && so.CheckLedgeHeight(height))
                {
                    selectedSo = so;
                    break;
                }
            }

            if (selectedSo == null)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
            }

            SetSubState(new PlayerParkourActionState(stateMachine, selectedSo));
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