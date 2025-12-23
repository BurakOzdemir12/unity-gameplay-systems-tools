using UnityEngine;

namespace _Project.Systems.Core.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State currentState;
        public State CurrentState => currentState;

        private State previousState;
        public State PreviousState => previousState;

        void Update()
        {
            currentState?.Tick(Time.deltaTime);
        }

        public void SwitchState(State newState)
        {
            currentState?.Exit();
            previousState = currentState;
            currentState = newState;
            currentState?.Enter();
        }
    }
}