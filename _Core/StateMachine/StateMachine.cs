using UnityEngine;

namespace _Project.Systems._Core.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State currentState;
        public State CurrentState => currentState;

        private State previousState;
        public State PreviousState => previousState;
        public State PreviousLeafState { get; internal set; }

        void Update()
        {
            // currentState?.Tick(Time.deltaTime);
            currentState?.UpdateStates(Time.deltaTime);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SwitchState(State newState)
        {
            currentState?.Exit();
            previousState = currentState;
            currentState = newState;
            currentState?.Enter();
        }
    }
}