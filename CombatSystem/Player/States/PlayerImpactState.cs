using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerImpactState : PlayerBaseState
    {
        private PlayerGroundedState GroundParent => GetSuperState() as PlayerGroundedState;
        
        public PlayerImpactState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private float remainingImpactTime;

        public override void Enter()
        {
            remainingImpactTime = stateMachine.PlayerConfigSo.CombatData.ImpactDuration;

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LightImpactHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            remainingImpactTime -= deltaTime;

            if (remainingImpactTime <= 0f)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
                return;
                // if (stateMachine.Targeter.SelectedTarget != null)
                // {
                //     GroundParent.SwitchSubState(new PlayerTargetingState(stateMachine));
                // }
                // else
                // {
                //     GroundParent.SwitchSubState(new PlayerFreeLookState(stateMachine));
                // }
                // stateMachine.DecideTargetOrLocomotion();
            }
        }

        public override void Exit()
        {
        }
    }
}