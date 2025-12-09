using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines
{
    public abstract class State
    {
        public abstract void Enter();
        public abstract void Tick(float deltaTime);
        public abstract void Exit();
    }
}