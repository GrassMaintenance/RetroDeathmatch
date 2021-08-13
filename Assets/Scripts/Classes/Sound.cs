using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {
	public string name;
	[HideInInspector] public AudioSource source;
	public AudioMixerGroup audioMixerGroup;
	[Range(0, 1)] public float volume;
	[Range(1f, 3f)] public int pitch;
	public AudioClip clip;
	public bool loop;
}
