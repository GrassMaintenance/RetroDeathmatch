using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    #region UI Components
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Toggle alwaysSprintToggle, fullScreenToggle, vSyncToggle, AmbientOcclusionToggle, BloomToggle, ChromaticAberrationToggle, MotionBlurToggle, VignetteToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown, resolutionDropDown;
    [SerializeField] private Slider defaultFOVSlider, mouseSensitivitySlider, masterVolumeSlider, musicVolumeSlider, gunVolumeSlider, menuVolumeSlider;
    [SerializeField] private TextMeshProUGUI sensitivityText, FOVText, masterVolumePercent, musicVolumePercent, gunVolumePercent, menuVolumePercent;
    [SerializeField] private AudioMixerGroup masterAudioMixer, musicAudioMixer, gunAudioMixer, menuAudioMixer;
    #endregion

    #region Toggle Struct
    [Serializable]
    public struct ToggleStruct {
        public string name;
        public Toggle toggle;
    }

    [SerializeField] ToggleStruct[] toggles;
    #endregion

    #region Dropdown Struct
    [Serializable]
    public struct DropdownStruct {
        public string name;

        public TMP_Dropdown dropdown;
    }

    [SerializeField]
    DropdownStruct[] dropdowns;
    #endregion

    #region Slider Struct
    [Serializable]
    public struct SliderStruct {
        public string name;
        public Slider slider;
        public UnityEngine.Object gameObject;
        public TextMeshProUGUI sliderValueText;
    }

    [SerializeField]
    SliderStruct[] sliders;
    #endregion

    [SerializeField] private PostProcessVolume postProcessVolume;
    private AmbientOcclusion ambientOcclusion = null;
    private Bloom bloom = null;
    private ChromaticAberration chromaticAberration = null;
    private MotionBlur motionBlur = null;
    private Vignette vignette = null;
    private Resolution resolution;
    private Resolution[] resolutions;
    public event EventHandler OnMouseSensitivityChanged;
    public static Settings Instance = null;

    private void Awake() {
        Instance = this;
        if(GetComponent<PlayerController>() == null) { gameObject.AddComponent<PlayerController>(); }
        LoadSettings();
        if(GetComponent<PlayerController>() != null) { Destroy(gameObject.GetComponent<PlayerController>()); }
    }

    public void SaveSettings() {
        //Save all slider settings
        foreach(SliderStruct slider in sliders) {
            PlayerPrefs.SetFloat(slider.name, slider.slider.value);
        }

        //Save all checkbox settings
        foreach(ToggleStruct toggle in toggles) {
            PlayerPrefs.SetInt(toggle.name + "Toggle", Convert.ToInt16(toggle.toggle.isOn));
        }

        //Save all dropdown settings
        foreach(DropdownStruct dropdown in dropdowns) {
            PlayerPrefs.SetInt(dropdown.name + "Dropdown", dropdown.dropdown.value);
        }

        //Write all settings to file in registry
        PlayerPrefs.Save();
    }

    public void LoadGUISettings() {
        LoadResolutions();
        LoadPostProcessingSettings();
        // Loads all checkboxes
        foreach(ToggleStruct toggle in toggles) {
            int toggleValue = PlayerPrefs.GetInt(toggle.name + "Toggle", 1);
            toggle.toggle.isOn = Convert.ToBoolean(toggleValue);
        }

        // Loads all dropdown values
        foreach(DropdownStruct dropdown in dropdowns) {
            int dropdownValue = PlayerPrefs.GetInt(dropdown.name + "Dropdown", 0);
            dropdown.dropdown.value = dropdownValue;
        }

        // Loads all slider settings
        foreach(SliderStruct slider in sliders) {
            float sliderValue = PlayerPrefs.GetFloat(slider.name, 1);

            if(slider.name.Contains("Volume")) {
                slider.slider.value = sliderValue;
                slider.sliderValueText.text = Mathf.Round(sliderValue * 100).ToString() + "%";

                if(gameObject.GetType() == typeof(AudioMixerGroup)) {
                    AudioMixerGroup mixerGroup = FindObjectsOfType<AudioMixerGroup>().FirstOrDefault(x => x.name == slider.name);
                    mixerGroup.audioMixer.SetFloat(slider.name, Mathf.Log10(sliderValue) * 20);
                }
            } else {
                slider.slider.value = sliderValue;
                slider.sliderValueText.text = sliderValue.ToString();
            }
        }
    }

    public void LoadSettings() {
        LoadGUISettings();
        SetAlwaysSprint(Convert.ToBoolean(PlayerPrefs.GetInt("AlwaysRun", 0)));
        SetFullscreen(Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenToggle", 0)));
        SetVSync(Convert.ToBoolean(PlayerPrefs.GetInt("VSyncToggle", 0)));
        SetQuality(PlayerPrefs.GetInt("QualityDropdown", 0));
        SetResolution(PlayerPrefs.GetInt("Resolution", 0));
        SetAmbientOcclusion(Convert.ToBoolean(PlayerPrefs.GetInt("AmbientOcclusionToggle", 0)));
        SetBloom(Convert.ToBoolean(PlayerPrefs.GetInt("BloomToggle", 0)));
        SetChromaticAbberation(Convert.ToBoolean(PlayerPrefs.GetInt("ChromaticAbberationToggle", 0)));
        SetMotionBlur(Convert.ToBoolean(PlayerPrefs.GetInt("MotionBlurToggle", 0)));
        SetVignette(Convert.ToBoolean(PlayerPrefs.GetInt("VignetteToggle", 0)));
    }

    private void LoadPostProcessingSettings() {
        postProcessVolume.profile.TryGetSettings(out ambientOcclusion);
        postProcessVolume.profile.TryGetSettings(out bloom);
        postProcessVolume.profile.TryGetSettings(out chromaticAberration);
        postProcessVolume.profile.TryGetSettings(out motionBlur);
        postProcessVolume.profile.TryGetSettings(out vignette);
    }

    private void LoadResolutions() {
        resolutionDropDown?.ClearOptions();
        resolutions = Screen.resolutions.Where(x => x.refreshRate == Screen.currentResolution.refreshRate).ToArray();
        if(resolutions.Length == 0) {
            resolutions = Screen.resolutions.Where(x => x.refreshRate == Screen.currentResolution.refreshRate - 1).ToArray();
        }
        List<string> options = new List<string>();
        for(int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.RefreshShownValue();
    }

    public void SetAlwaysSprint(bool isEnabled) {
        alwaysSprintToggle.isOn = isEnabled;
    }

    public void SetMouseSensitivity(float value) {
        sensitivityText.text = value.ToString();
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
        OnMouseSensitivityChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetDefaultFOV(float value) {
        FOVText.text = value.ToString();
    }

    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVSync(bool enableVSync) {
        QualitySettings.vSyncCount = Convert.ToInt32(enableVSync);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex) {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, Screen.currentResolution.refreshRate);
    }

    public void SetMasterVolume(float volume) {
        masterAudioMixer.audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        masterVolumePercent.text = ConvertToPercent(volume);
    }

    public void SetMenuVolume(float volume) {
        menuAudioMixer.audioMixer.SetFloat("Menu", Mathf.Log10(volume) * 20);
        menuVolumePercent.text = ConvertToPercent(volume);
    }

    public void SetGunVolume(float volume) {
        gunAudioMixer.audioMixer.SetFloat("Guns", Mathf.Log10(volume) * 20);
        gunVolumePercent.text = ConvertToPercent(volume);
    }

    public void SetMusicVolume(float volume) {
        musicAudioMixer.audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        musicVolumePercent.text = ConvertToPercent(volume);
    }

    public void SetAmbientOcclusion(bool isEnabled) {
        ambientOcclusion.enabled.value = isEnabled;
    }

    public void SetBloom(bool isEnabled) {
        bloom.enabled.value = isEnabled;
    }

    public void SetChromaticAbberation(bool isEnabled) {
        chromaticAberration.enabled.value = isEnabled;
    }

    public void SetMotionBlur(bool isEnabled) {
        motionBlur.enabled.value = isEnabled;
    }

    public void SetVignette(bool isEnabled) {
        vignette.enabled.value = isEnabled;
    }

    private string ConvertToPercent(float volume) {
        float _volume = Mathf.Pow(10, Mathf.Log10(volume)) * 100;
        return Mathf.Round(_volume).ToString() + "%";
    }
}
