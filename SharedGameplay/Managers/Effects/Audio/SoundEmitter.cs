using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace _Project.Systems.SharedGameplay.Managers.Effects.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        private AudioSource audioSource;
        private Coroutine playingCoroutine;
        private IObjectPool<SoundEmitter> pool;

        public LinkedListNode<SoundEmitter> Node { get; set; }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Initialize(SoundData data, IObjectPool<SoundEmitter> poolRef)
        {
            this.pool = poolRef;
            transform.position = data.Position;

            audioSource.clip = data.Clip;
            audioSource.volume = data.Volume;
            audioSource.pitch = data.Pitch;
            audioSource.spatialBlend = data.SpatialBlend;
            audioSource.loop = data.Loop;

            audioSource.minDistance = 1f;
            audioSource.maxDistance = 25f;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        public void Play()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }

            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundEnd());
        }

        private IEnumerator WaitForSoundEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            Stop();
        }

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            audioSource.Stop();

            pool?.Release(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }
    }
}