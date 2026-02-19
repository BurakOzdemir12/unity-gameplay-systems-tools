using UnityEngine;

namespace _Project.Systems.SharedGameplay.Managers.Effects.Audio
{
    public struct SoundData
    {
        public AudioClip Clip;
        public Vector3 Position;
        public float Volume;
        public float Pitch;
        public float SpatialBlend;
        public bool FrequentSound;
        public bool Loop;

        public SoundData(AudioClip clip, Vector3 position, float volume, float pitch, float spatialBlend,
            bool frequentSound, bool loop)
        {
            Clip = clip;
            Position = position;
            Volume = volume;
            Pitch = pitch;
            SpatialBlend = spatialBlend;
            FrequentSound = frequentSound;
            Loop = loop;
        }
    }
}