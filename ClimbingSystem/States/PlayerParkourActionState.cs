using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.ClimbingSystem.Structs;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.States
{
    public class PlayerParkourActionState : PlayerBaseState
    {
        private string animTag;
        private bool hasMatchedTarget;
        private readonly ClimbTypeDataSo so;
        private readonly ParkourDecision decision;
        private LedgeHitData hitData;
        private Vector3 normal;

        public PlayerParkourActionState(PlayerStateMachine stateMachine, ClimbTypeDataSo so, ParkourDecision decision) :
            base(stateMachine)
        {
            this.so = so;
            this.decision = decision;
        }

        public override void Enter()
        {
            hitData = stateMachine.ClimbController.CurrentLedgeHitData;
            normal = hitData.ForwardRayHitInfo.normal;
            normal.y = 0f;

            Vector3 toHit = hitData.ForwardRayHitInfo.point - stateMachine.transform.position;
            toHit.y = 0f;

            if (Vector3.Dot(normal.normalized, toHit.normalized) > 0f)
                normal = -normal;

            animTag = so.AnimTag;
            stateMachine.Animator.applyRootMotion = true;
            // stateMachine.Controller.enabled = false;

            stateMachine.Animator.SetBool(stateMachine.Mirror, decision.Mirror);

            stateMachine.Animator.CrossFadeInFixedTime(so.AnimHash, so.ClimbCrossFadeDuration);
            hasMatchedTarget = false;
        }

        public override void Tick(float deltaTime)
        {
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, animTag);
            if (normalizedTime > 1f)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
            }

            RotateTowardsObstacle(deltaTime, normalizedTime);

            MatchTarget(normalizedTime, deltaTime);
        }

        private void RotateTowardsObstacle(float deltaTime, float normalizedTime)
        {
            if (so.rotateToObstacle && normalizedTime < 1f)
            {
                Quaternion matchRotation = Quaternion.LookRotation(-normal.normalized, Vector3.up);
                stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation,
                    matchRotation, so.rotationSpeed * deltaTime);
            }
        }

        private void MatchTarget(float normalizedTime, float deltaTime)
        {
            if (!so.enableTargetMatching) return;

            if (stateMachine.Animator.isMatchingTarget) return;
            if (hasMatchedTarget) return;
            if (normalizedTime < so.MatchStartTime || normalizedTime > so.MatchTargetTime) return;

            var hitData = stateMachine.ClimbController.CurrentLedgeHitData;
            Vector3 matchPos = hitData.HeightRayHitInfo.point;


            Vector3 soWeight = so.MatchPosWeight;
            MatchTargetWeightMask weightMask = new MatchTargetWeightMask(soWeight, 0);
            // MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 0);

            stateMachine.Animator.MatchTarget(
                matchPos,
                stateMachine.transform.rotation,
                decision.MatchBodyPart,
                weightMask,
                so.MatchStartTime,
                so.MatchTargetTime
            );
            hasMatchedTarget = true;
        }

        public override void Exit()
        {
            stateMachine.Animator.applyRootMotion = false;
            // stateMachine.Controller.enabled = true;

            stateMachine.Animator.SetBool(stateMachine.Mirror, false);
        }
    }
}