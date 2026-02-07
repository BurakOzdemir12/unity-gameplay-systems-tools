using _Project.Systems._Core.Enums;
using _Project.Systems._Core.EventBus;
using _Project.Systems._Core.EventBus.Events;
using _Project.Systems.EnvironmentSystem.ScriptableObjects;
using _Project.Systems.EnvironmentSystem.Time.Enums;
using _Project.Systems.EnvironmentSystem.Time.Events;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using _Project.Systems.EnvironmentSystem.Weather.Events;
using _Project.Systems.SharedGameplay.Feedback;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.Managers.Effects.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

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
        private AudioSource audioSource;

        [SerializeField] private AudioSource ambientAudioSource;
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
            audioSource = GetComponent<AudioSource>();
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

            audioSource.PlayOneShot(clip, volume);

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

            audioSource.PlayOneShot(clip, volume);
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

            audioSource.PlayOneShot(clip, volume);

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

            audioSource.PlayOneShot(clip, volume);
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
            audioSource.PlayOneShot(clip, volume);
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

        public void PlayShieldBreak(AudioClip clip, float volume)
        {
            audioSource.PlayOneShot(clip, volume);
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