using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;
    public Sound[] sounds;

    private void Awake() => Instance = this;

    private void Start() {
        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.name = s.name;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        sound?.source.Play();
    }
}
