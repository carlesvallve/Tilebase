using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoSingleton <AudioManager> {

	public Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
	public float generalVolume = 1.0f;


	public void Play(string wav, float volume = 1.0f, bool loop = false) {
		Play(wav, Vector3.zero, volume, 1.0f, loop);
	}


	public void Play(string wav, float volume, float pitch, bool loop = false) {
		Play(wav, Vector3.zero, volume, pitch, loop);
	}


	public void Play(string wav, Vector3 pos, float volume = 1.0f, float pitch = 1.0f, bool loop = false) {
		AudioClip clip = Resources.Load(wav) as AudioClip;

		// Log error if clip could not be loaded
		if (!clip) {
			Debug.LogError("Error while loading audio clip --> " + wav);
			return;
		}

		Play(clip, pos, volume, pitch, loop);
	}


	public void Play(AudioClip clip, float volume = 1.0f, float pitch = 1.0f, bool loop = false) {
		Play(clip, Vector3.zero, volume, pitch, loop);
	}


	public void Play(AudioClip clip, Vector3 pos, float volume = 1.0f, float pitch = 1.0f, bool loop = false) {
		// Add the audio source component
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.loop = loop;

		// Log error if source could not be created
		if(!source) {
			print("Error while creating audio source component!");
			return;
		}

		// Set audio source props
		source.clip = clip;
		source.volume = volume * generalVolume;
		source.pitch = pitch;

		// Play it
		source.Play();

		// If is not looping, destroy the source component after the sound has played
		if (!loop)  {
			Destroy(source, clip.length);
		}
	}


	public void Stop(string wav) {
		if (audioSources.ContainsKey(wav)) {
			// Stop source and remove its component
			AudioSource source = audioSources[wav];
			source.Stop();
			Destroy(source);

			// Remove audio source from sources dictionary
			audioSources.Remove(wav);

		} else {

			// Log error if source could not be removed
			Debug.LogError("Error while stopping AudioSource --> " + wav);
		}
	}

}
