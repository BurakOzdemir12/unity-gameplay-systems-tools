using System;
using _Project.Core.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerStateMachine : StateMachine
    {
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field:SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField] public float locomotionAnimatorDampTime=0.1f;
        [field: SerializeField] public float rotationDampTime=0.1f;
        [field: SerializeField] public float FreeMovementSpeed { get; private set; }
        public Transform MainCameraTransform { get; private set; }
        public readonly int FreeLookSpeedParam = Animator.StringToHash("FreeLookSpeed");

        private void Start()
        {
            if (UnityEngine.Camera.main == null) Debug.LogError("No main camera found!");
            if (UnityEngine.Camera.main != null) MainCameraTransform = UnityEngine.Camera.main.transform;
            SwitchState(new PlayerFreeLookState(this));
        }
    }
}