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
        private ClimbTypeDataSo selectedSo;

        public PlayerClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            float height = stateMachine.ClimbController.CurrentLedgeHitData.heightRayHitInfo.point.y
                           - stateMachine.transform.position.y;

            selectedSo = null;

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
            // #region Rotation To Obstacle -normals
            //
            // if (so.rotateToObstacle)
            // {
            //     Vector3 normal = hitData.forwardRayHitInfo.normal;
            //     Quaternion rotationToObstacle = Quaternion.LookRotation(-normal);
            //     float maxDegreesDelta = so.rotationSpeed * deltaTime;
            //
            //     stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation,
            //         rotationToObstacle,
            //         maxDegreesDelta);
            // }
            //
            // #endregion


        }

        public override void Exit()
        {
            ClearSubState();
        }

        public void SwitchSubState(PlayerBaseState newSubState) => SetSubState(newSubState);
    }
}