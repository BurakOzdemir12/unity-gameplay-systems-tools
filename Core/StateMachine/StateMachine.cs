using UnityEngine;

namespace _Project.Systems.Core.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State currentState;
        public State CurrentState => currentState;

        void Update()
        {
            currentState?.Tick(Time.deltaTime);
        }

        public void SwitchState(State newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }
    }
}