using System;
using System.Collections.Generic;
using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems.CombatSystem.Events;
using _Project.Systems.EnvironmentSystem.ScriptableObjects;
using _Project.Systems.EnvironmentSystem.Time.Enums;
using _Project.Systems.EnvironmentSystem.Time.Events;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using _Project.Systems.EnvironmentSystem.Weather.Events;
using _Project.Systems.MovementSystem.Events;
using _Project.Systems.SharedGameplay.Feedback;
using _Project.Systems.SharedGameplay.Managers.Effects.Audio.Enums;
using _Project.Systems.SharedGameplay.Managers.Effects.Audio.Structs;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Project.Systems.SharedGameplay.Managers.Effects.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }


        [Header("Audio Mixer Groups")] [SerializeField]
        private AudioMixer mainMixer;

        [Space(5)]
        [Tooltip("Mixer Group for Impact Sounds like Sword hit enemy, rock or pickaxe hit on rock, etc.")]
        [SerializeField]
        private AudioMixerGroup impactMixerGroup;

        [Tooltip("Mixer Group for Traversal Sounds like footsteps, jump, landing, etc.")] [SerializeField]
        private AudioMixerGroup traversalMixerGroup;

        [Tooltip("Mixer Group for Combat Vocal Sounds like grunts, yells, etc.")] [SerializeField]
        private AudioMixerGroup combatSFxMixerGroup;

        [Tooltip("Mixer Group for Combat Sounds like sword swing, bow shoot, etc.")] [SerializeField]
        private AudioMixerGroup combatVocalMixerGroup;

        [Tooltip("Mixer Group for Dialogue Sounds like NPC conversations, etc.")] [SerializeField]
        private AudioMixerGroup dialogueMixerGroup;

        [Tooltip("Mixer Group for Gathering Sounds like harvesting crops, mining, etc.")] [SerializeField]
        private AudioMixerGroup gatheringMixerGroup;

        [Tooltip("Mixer Group for Environmental Sounds like rain, wind, etc.")] [SerializeField]
        private AudioMixerGroup envMixerGroup;

        [Header("Pool Settings")] [SerializeField]
        private SoundEmitter soundEmitterPrefab;

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxSoundInstances = 30;

        [SerializeField] private Transform poolContainer;
        private IObjectPool<SoundEmitter> soundEmitterPool;
        private readonly List<SoundEmitter> activeSoundEmitters = new List<SoundEmitter>();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new LinkedList<SoundEmitter>();

        #region Impact Events

        private EventBinding<CharacterTraversalEvent> interactionBinding;
        private EventBinding<CharacterCombatActionEvent> combatBinding;
        private EventBinding<CharacterGatheringActionEvent> gatheringBinding;
        private EventBinding<WeaponImpactActionEvent> weaponImpactBinding;
        private EventBinding<ToolImpactActionEvent> toolImpactBinding;

        #endregion

        #region Time Events

        private EventBinding<TimeChangedEvent> timeChangedBinding;
        private EventBinding<WeatherChangedEvent> weatherChangedBinding;

        #endregion

        [Header("Audio Sources")] [SerializeField]
        private AudioSource ambientAudioSource;

        [SerializeField] private AudioSource musicAudioSource;

        [Space(2)] [SerializeField] private EnvironmentalAudioProfile envAudioProfile;
        [SerializeField] private GameMusicProfile musicAudioProfile;

        [Header("Current State")] private DivisionsOfDay currentDivision = DivisionsOfDay.Morning;
        private WeatherType currentWeather = WeatherType.Clear;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            InitializePool();
        }


        private void OnEnable()
        {
            #region Impact Events

            interactionBinding = new EventBinding<CharacterTraversalEvent>(HandleTraversalEvent);
            EventBus<CharacterTraversalEvent>.Subscribe(interactionBinding);

            combatBinding = new EventBinding<CharacterCombatActionEvent>(HandleCombatActionEvent);
            EventBus<CharacterCombatActionEvent>.Subscribe(combatBinding);

            gatheringBinding = new EventBinding<CharacterGatheringActionEvent>(HandleGatheringActionEvent);
            EventBus<CharacterGatheringActionEvent>.Subscribe(gatheringBinding);

            weaponImpactBinding = new EventBinding<WeaponImpactActionEvent>(HandleWeaponImpactEvent);
            EventBus<WeaponImpactActionEvent>.Subscribe(weaponImpactBinding);

            toolImpactBinding = new EventBinding<ToolImpactActionEvent>(HandleToolImpactEvent);
            EventBus<ToolImpactActionEvent>.Subscribe(toolImpactBinding);

            #endregion

            #region Time Events

            timeChangedBinding = new EventBinding<TimeChangedEvent>(HandleTimeChangedEvent);
            EventBus<TimeChangedEvent>.Subscribe(timeChangedBinding);

            weatherChangedBinding = new EventBinding<WeatherChangedEvent>(HandleWeatherChangedEvent);
            EventBus<WeatherChangedEvent>.Subscribe(weatherChangedBinding);

            #endregion
        }


        private void OnDisable()
        {
            EventBus<CharacterTraversalEvent>.Unsubscribe(interactionBinding);
            EventBus<CharacterCombatActionEvent>.Unsubscribe(combatBinding);
            EventBus<CharacterGatheringActionEvent>.Unsubscribe(gatheringBinding);
            EventBus<WeaponImpactActionEvent>.Unsubscribe(weaponImpactBinding);
            EventBus<ToolImpactActionEvent>.Unsubscribe(toolImpactBinding);
            EventBus<TimeChangedEvent>.Unsubscribe(timeChangedBinding);
            EventBus<WeatherChangedEvent>.Unsubscribe(weatherChangedBinding);
        }

        private void Start()
        {
            PlayAmbientSound(envAudioProfile.GetEnvAudio(currentDivision, currentWeather));
            PlayMusicTrack(musicAudioProfile.GetMusicTrack(false, currentDivision));
        }
        // public SoundBuilder CreateSoundBuilder() => new SoundBuilder(this);
        //
        // public bool CanPlaySound(SoundData data) {
        //     if (!data.FrequentSound) return true;
        //
        //     if (FrequentSoundEmitters.Count >= maxSoundInstances) {
        //         try {
        //             FrequentSoundEmitters.First.Value.Stop();
        //             return true;
        //         } catch {
        //             Debug.Log("SoundEmitter is already released");
        //         }
        //         return false;
        //     }
        //     return true;
        // }

        public SoundEmitter GetSoundEmitter()
        {
            return soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter emitter)
        {
            soundEmitterPool.Release(emitter);
        }

        public void StopAllSounds()
        {
            LinkedList<SoundEmitter> tempList = new LinkedList<SoundEmitter>(activeSoundEmitters);
            foreach (var emitter in tempList)
            {
                emitter.Stop();
            }

            FrequentSoundEmitters.Clear();
        }

        private void InitializePool()
        {
            if (poolContainer == null) poolContainer = new GameObject("SoundEmitter_Pool").transform;
            poolContainer.SetParent(this.transform);

            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                defaultCapacity,
                maxPoolSize
            );
        }

        private SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(soundEmitterPrefab, poolContainer);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        private void OnTakeFromPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(emitter);
        }

        private void OnReturnedToPool(SoundEmitter emitter)
        {
            if (emitter.Node != null)
            {
                FrequentSoundEmitters.Remove(emitter.Node);
                emitter.Node = null;
            }

            emitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(emitter);
        }

        private void OnDestroyPoolObject(SoundEmitter emitter)
        {
            if (emitter != null && emitter.gameObject != null)
            {
                Destroy(emitter.gameObject);
            }
        }

        #region Core Play Logic (With Pool pattern)

        private void PlaySound(SoundData data)
        {
            if (data.Clip == null) return;
            if (data.FrequentSound && FrequentSoundEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }

            SoundEmitter emitter = soundEmitterPool.Get();
            emitter.Initialize(data, soundEmitterPool);
            emitter.Play();

            if (data.FrequentSound)
            {
                emitter.Node = FrequentSoundEmitters.AddLast(emitter);
            }
        }

        #endregion

        #region Mixer Group Selection

        private AudioMixerGroup GetMixerGroup(SoundChannel channel)
        {
            return channel switch
            {
                SoundChannel.Traversal => traversalMixerGroup,
                SoundChannel.Impact => impactMixerGroup,
                SoundChannel.CombatVocal => combatVocalMixerGroup,
                SoundChannel.Dialogue => dialogueMixerGroup,
                SoundChannel.Combat => combatSFxMixerGroup,
                SoundChannel.Gathering => gatheringMixerGroup,
                SoundChannel.Environment => envMixerGroup,
                _ => null
            };
        }

        #endregion

        #region Event Bus Handlers

        private void HandleTraversalEvent(CharacterTraversalEvent @evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetTraversalFeedback(
                    evt.Surface,
                    evt.Type,
                    evt.ActionTag,
                    out var clip,
                    out _,
                    out var volume))
                return;

            PlaySound(new SoundData
            {
                Clip = clip, Position = evt.Position, Volume = volume, Pitch = Random.Range(0.9f, 1.1f),
                SpatialBlend = 1f, FrequentSound = true, Loop = false, MixerGroup = traversalMixerGroup
            });
            // audioSource.PlayOneShot(clip, volume);
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }

        private void HandleCombatActionEvent(CharacterCombatActionEvent evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;
            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetCombatActionFeedback(
                    evt.Surface,
                    evt.Type,
                    evt.WeaponType,
                    evt.ActionTag,
                    out var clip,
                    out _,
                    out var volume))
                return;

            PlaySound(new SoundData
            {
                Clip = clip, Position = evt.Position, Volume = volume, Pitch = Random.Range(0.9f, 1.1f),
                SpatialBlend = 1f, FrequentSound = true, Loop = false, MixerGroup = combatSFxMixerGroup
            });
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }

        //TODO Create HandleLootActionEvent for impacts and Un/Subscribe
        private void HandleWeaponImpactEvent(WeaponImpactActionEvent evt)
        {
            var weaponData = evt.WeaponData;
            if (weaponData == null) return;

            var profile = weaponData.weaponImpactFeedbackProfile;
            if (profile == null) return;

            WeaponType currentWeaponType = weaponData.weaponType;

            if (!profile.TryGetWeaponImpactActionFeedback(
                    evt.Surface,
                    currentWeaponType,
                    evt.Tag,
                    out var clip,
                    out _,
                    out var volume
                )) return;

            PlaySound(new SoundData
            {
                Clip = clip, Position = evt.Position, Volume = volume, Pitch = Random.Range(0.9f, 1.1f),
                SpatialBlend = 1f, FrequentSound = true, Loop = false, MixerGroup = impactMixerGroup
            });
            // AudioSource.PlayClipAtPoint(clip, evt.Position, volume);
        }

        private void HandleToolImpactEvent(ToolImpactActionEvent evt)
        {
            var toolData = evt.ToolData;
            if (toolData == null) return;

            var profile = toolData.toolImpactFeedbackProfile;
            if (profile == null) return;

            ToolType currentToolType = toolData.toolType;

            if (!profile.TryGetToolImpactActionFeedback(
                    evt.Surface,
                    currentToolType,
                    evt.Tag,
                    out var clip, out _, out var volume
                )) return;

            PlaySound(new SoundData
            {
                Clip = clip, Position = evt.Position, Volume = volume, Pitch = Random.Range(0.9f, 1.1f),
                SpatialBlend = 1f, FrequentSound = true, Loop = false, MixerGroup = impactMixerGroup
            });
        }

        private void HandleGatheringActionEvent(CharacterGatheringActionEvent evt)
        {
            if (!evt.Source.TryGetComponent(out CharacterFeedbackProfileHolder holder)) return;

            var profile = holder.Profile;
            if (profile == null) return;

            if (!profile.TryGetGatherActionFeedback(
                    evt.Type,
                    evt.ToolType, evt.ActionTag,
                    out var clip, out _, out var volume
                )) return;
            PlaySound(new SoundData
            {
                Clip = clip, Position = evt.Position, Volume = volume, Pitch = Random.Range(0.9f, 1.1f),
                SpatialBlend = 1f, FrequentSound = true, Loop = false, MixerGroup = gatheringMixerGroup
            });
        }

        private void HandleTimeChangedEvent(TimeChangedEvent evt)
        {
            if (!evt.IsDivisionJustChanged) return;
            currentDivision = evt.Division;
            AudioClip clipToPlay = envAudioProfile.GetEnvAudio(evt.Division, currentWeather);

            PlayAmbientSound(clipToPlay);
        }

        private void HandleWeatherChangedEvent(WeatherChangedEvent evt)
        {
            currentWeather = evt.CurrentWeatherType;
            AudioClip clipToPlay = envAudioProfile.GetEnvAudio(currentDivision, evt.CurrentWeatherType);

            PlayAmbientSound(clipToPlay);
        }

        #endregion

        #region Singleton Function Calls

        public void PlayGeneric3DSound(AudioClip clip, Vector3 position, SoundChannel channel, float volume = 1f,
            bool isFrequent = true,
            bool isLoop = false)
        {
            PlaySound(new SoundData
            {
                Clip = clip, Position = position, Volume = volume, Pitch = 1f, SpatialBlend = 1f,
                FrequentSound = isFrequent, Loop = isLoop, MixerGroup = GetMixerGroup(channel)
            });
        }

        #endregion

        private void PlayAmbientSound(AudioClip clip)
        {
            if (clip == null) return;

            ambientAudioSource.Stop();
            ambientAudioSource.clip = clip;
            ambientAudioSource.loop = true;
            ambientAudioSource.Play();
        }

        private void PlayMusicTrack(AudioClip clip)
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = clip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }
}