using _Project.Systems.MovementSystem.Player.Enums;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;

namespace _Project.Systems.MovementSystem.Player.States
{
    public class PlayerLandingState : PlayerBaseState
    {
        private const string LANDING_TAG = "Landing";
        private float normalisedTime;
        private readonly LandingType landingType;

        public PlayerLandingState(PlayerStateMachine stateMachine, LandingType landingType) : base(stateMachine)
        {
            this.landingType = landingType;
        }

        public override void Enter()
        {
            int hash = landingType == LandingType.Light ? stateMachine.LandingHash : stateMachine.LandingHeavyHash;
            stateMachine.Animator.CrossFadeInFixedTime(hash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            normalisedTime = GetNormalizedTime(stateMachine.Animator, 0, LANDING_TAG);


            if (normalisedTime >= stateMachine.PlayerConfigSo.FallLandData.LandingStateExitTime)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
            }
        }

        public override void Exit()
        {
        }

        // private bool IsLandingWindowActive()
        // {
        //     return normalisedTime >= stateMachine.LandingAnimStartTime &&
        //            normalisedTime <= stateMachine.LandingAnimEndTime;
        // }
    }
}