using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.States
{
    public class PlayerClimbUpState : PlayerBaseState
    {
        private const string CLIMB_UP_TAG = "ClimbUp";
        private Vector3 surfacePoint;
        private Vector3 groundPoint;
        private Vector3 finalStandPos;

        private ClimbTypeDataSo data;

        public PlayerClimbUpState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        // BracedHangToCrouchClimbHash
        //     FreeHangClimbHash
        public override void Enter()
        {
            data = stateMachine.PlayerConfigSo.ClimbTypeDataSet[0];
            if (stateMachine.IsFreeFlowClimb)
            {
                stateMachine.Animator.CrossFadeInFixedTime(data.ClimbUpAnimHash,
                    stateMachine.CrossFadeDuration);
                return;
            }

            stateMachine.Animator.CrossFadeInFixedTime(data.FreeHangClimbHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            stateMachine.transform.position =
                Vector3.Slerp(stateMachine.transform.position, finalStandPos, 2f * deltaTime);
            float normalisedTime = GetNormalizedTime(stateMachine.Animator, 0, CLIMB_UP_TAG);
            if (normalisedTime > 1f)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}