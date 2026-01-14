using UnityEngine;

namespace _Project.Systems._Core.StateMachine
{
    public abstract class State
    {
        protected readonly StateMachine stateMachine;

        protected State superState;
        protected State subState;

        protected State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public abstract void Enter();


        public abstract void Tick(float deltaTime);
        public abstract void Exit();

        public void UpdateStates(float deltaTime)
        {
            Tick(deltaTime);
            subState?.UpdateStates(deltaTime);
        }

        public State GetLeafState()
        {
            State s = this;
            while (s.subState != null) s = s.subState;
            return s;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected void SetSubState(State newSubState)
        {
            if (subState != null)
            {
                stateMachine.PreviousLeafState = subState.GetLeafState();
            }
            else
            {
                stateMachine.PreviousLeafState = this;
            }

            subState?.Exit();
            subState = newSubState;
            subState.superState = this;
            subState.Enter();
        }

        protected void ClearSubState()
        {
            subState?.Exit();
            subState = null;
        }

        protected void SwitchRootState(State newRootState)
        {
            stateMachine.SwitchState(newRootState);
        }

        public State GetSubState() => subState;
        public State GetSuperState() => superState;

        protected float GetNormalizedTime(Animator animator, int layerIndex, string tag)
        {
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(layerIndex);

            if (animator.IsInTransition(layerIndex) && nextInfo.IsTag(tag))
            {
                return nextInfo.normalizedTime;
            }
            else if (!animator.IsInTransition(layerIndex) && currentInfo.IsTag(tag))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }
    }
}