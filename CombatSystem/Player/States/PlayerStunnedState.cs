using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerStunnedState : PlayerBaseState
    {
        private float stunTime;

        private PlayerGroundedState GroundParent => GetSuperState() as PlayerGroundedState;

        public PlayerStunnedState(PlayerStateMachine stateMachine, float duration) : base(stateMachine)
        {
            stunTime = duration;
        }

        public override void Enter()
        {
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.PlayerConfigSo.CombatData.StunnedAnimParamHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            stunTime -= deltaTime;
            if (stunTime <= 0)
            {
                GroundParent?.SwitchSubState(new PlayerFreeLookState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}