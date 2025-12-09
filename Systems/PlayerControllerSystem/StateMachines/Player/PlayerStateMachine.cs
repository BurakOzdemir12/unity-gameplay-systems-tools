using System;
using _Project.Core.Scripts;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerStateMachine : StateMachine
    {
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [field:SerializeField] public CharacterController Controller { get; private set; }
        [field:SerializeField] public Animator Animator { get; private set; }
        [field:SerializeField] public string FreeLookSpeedParam { get; private set; }
        [field:SerializeField] public float FreeMovementSpeed { get; private set; }
        private void Start()
        {
            SwitchState(new PlayerTestState(this));
        }
    }
}