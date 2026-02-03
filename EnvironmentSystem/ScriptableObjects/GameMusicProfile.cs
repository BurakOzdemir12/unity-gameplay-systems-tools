using _Project.Systems.EnvironmentSystem.Time.Enums;
using UnityEngine;

namespace _Project.Systems.EnvironmentSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameMusicProfile", menuName = "Scriptable Objects/Audio Profile/Game Music Profile")]
    public class GameMusicProfile : ScriptableObject
    {
        [Header("Ambient Tracks by place")] public AudioClip[] dayVillageTracks;
        [Header("Exploration Ambient Tracks")] public AudioClip[] dayExplorationTracks;
        public AudioClip[] nightExplorationTracks;
        [Header("State tracks")] public AudioClip[] dangerTracks;
        public AudioClip[] bossTracks;

        public AudioClip GetMusicTrack(bool isDanger, DivisionsOfDay time)
        {
            if (isDanger) return GetRandom(dangerTracks);

            return time switch
            {
                DivisionsOfDay.Night or DivisionsOfDay.Evening => GetRandom(nightExplorationTracks),
                _ => GetRandom(dayExplorationTracks)
            };
        }

        private AudioClip GetRandom(AudioClip[] clips)
        {
            if (clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }
    }
}