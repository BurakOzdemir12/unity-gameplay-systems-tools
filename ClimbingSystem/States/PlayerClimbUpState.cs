using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.States
{
    public class PlayerClimbUpState : PlayerBaseState
    {
        private const string CLIMB_UP_TAG = "ClimbUp";
        private Vector3 surfacePoint;
        private Vector3 groundPoint;
        private Vector3 finalStandPos;


        public PlayerClimbUpState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        // BracedHangToCrouchClimbHash
        //     FreeHangClimbHash
        public override void Enter()
        {

            if (stateMachine.IsFreeFlowClimb)
            {
                stateMachine.Animator.CrossFadeInFixedTime(stateMachine.BracedHangToCrouchClimbHash,
                    stateMachine.CrossFadeDuration);
                return;
            }

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.FreeHangClimbHash, stateMachine.CrossFadeDuration);
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