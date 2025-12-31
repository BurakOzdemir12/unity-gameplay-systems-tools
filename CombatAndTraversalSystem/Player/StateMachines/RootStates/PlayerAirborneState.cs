using _Project.Systems.CombatAndTraversalSystem.Player.Enums;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates
{
   
    public class PlayerAirborneState : PlayerBaseState
    {
        private float startY;
        private float fallDistance;
        private bool fromJumping;


        public PlayerAirborneState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            startY = stateMachine.transform.position.y;
            fromJumping = stateMachine.ConsumeJump();

            // if (stateMachine.ClimbController.HasValidLedge)
            // {
            //     
            // }
            if (fromJumping)
            {
                SetSubState(new PlayerJumpingState(stateMachine));
            }
            else
            {
                // if (TryEnterClimb()) return;

                SetSubState(new PlayerFallingState(stateMachine));
            }
        }

        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            if (!stateMachine.Controller.isGrounded) return;

            fallDistance = startY - stateMachine.transform.position.y;



            if (!fromJumping && fallDistance < stateMachine.LandingHeightThreshold)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
            }


            LandingType landingType = (fallDistance >= stateMachine.LandingHardHeightThreshold)
                ? LandingType.Heavy
                : LandingType.Light;

            if (subState is not PlayerLandingState)
                SetSubState(new PlayerLandingState(stateMachine, landingType));
        }

        public override void Exit()
        {
            ClearSubState();
        }

        public void SwitchSubState(PlayerBaseState newSubState) => SetSubState(newSubState);
    }
}

        //
        // public override void Tick(float deltaTime)
        // {
        //     Move(deltaTime);
        //
        //     float radius = stateMachine.Controller.radius * 0.9f;
        //
        //     float landingProbeDistance = Mathf.Max(0.6f, stateMachine.Controller.stepOffset + 0.2f);
        //
        //     bool groundedByCC = stateMachine.Controller.isGrounded;
        //     bool groundBelowByProbe = ProbeGroundBelow(out _, 0.6f, radius, upOffset: 0.1f);
        //
        //     if (!groundedByCC && !groundBelowByProbe)
        //         return;
        //
        //     fallDistance = startY - stateMachine.transform.position.y;
        //
        //     if (!fromJumping && fallDistance < stateMachine.LandingHeightThreshold)
        //     {
        //         SwitchRootState(new PlayerGroundedState(stateMachine));
        //         return;
        //     }
        //
        //     LandingType landingType = (fallDistance >= stateMachine.LandingHardHeightThreshold)
        //         ? LandingType.Heavy
        //         : LandingType.Light;
        //
        //     if (subState is not PlayerLandingState)
        //         SetSubState(new PlayerLandingState(stateMachine, landingType));
        // }
