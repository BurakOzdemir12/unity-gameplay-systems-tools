using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using UnityEngine;

namespace _Project.Systems._Core.Effects.Audio
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        private AudioClip footstepClip;


        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
        }


        private void PlayOneShot(AudioSource source)
        {
        }

        private void OnDisable()
        {
        }
    }
}