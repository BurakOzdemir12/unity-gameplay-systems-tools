using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;

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
                stateMachine.SwitchState(new PlayerGroundedState(stateMachine));
                // GroundParent?.SwitchSubState(new PlayerFreeLookState(stateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}