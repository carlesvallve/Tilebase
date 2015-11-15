// .Net includes
using System;
using System.Collections;

// Unity includes
using UnityEngine;

namespace WizUtils {

	/// <summary>
	/// Audio manager for use when playing standard BGM, looped BGM, and 16 canal SFX
	/// </summary>
	public class AudioManager : SingletonComponent<AudioManager> {

		// Sound Effects
		private const int SFX_MAX_CANALS = 16;
		private int SFXCurrentCanal = 0;
		private AudioSource[] audioSourceSFX;

		// BGM
		private AudioSource musicIntro;
		private AudioSource musicLoop;
		private bool playingBGM;

		// Volume settings
		private float backgroundVolume;
		private float SFXVolume;

		/// <summary>
		/// When awake initialise audio sources to use
		/// </summary>
		void Awake() {
			musicIntro = gameObject.AddComponent<AudioSource>() as AudioSource;
			musicLoop = gameObject.AddComponent<AudioSource>() as AudioSource;

			audioSourceSFX = new AudioSource[SFX_MAX_CANALS];

			for (int i = 0; i < SFX_MAX_CANALS; i++) {
				audioSourceSFX[i] = gameObject.AddComponent<AudioSource>() as AudioSource;
				audioSourceSFX[i].loop = false;
			}
		}

		/// <summary>
		/// Set defaults
		/// </summary>
		void Start() {
			backgroundVolume = 1;
			SFXVolume = 1;
		}

		//****************************************************
		// BACKGROUND MUSIC API
		//****************************************************

		/// <summary>
		/// Play standard background music from Resources
		/// </summary>
		/// <param name="fileLocation">Full file path in resources</param>
		public void PlayBGM(string fileLocation) {
			AudioClip bgm = null;
			if (fileLocation != null) { 
				bgm = (AudioClip)Resources.Load(fileLocation); 
			}

			PlayBGM(null, bgm);
		}

		/// <summary>
		/// Overload to play loop from Resources
		/// </summary>
		/// <param name="startFileName">Full file path in resources for the start of the music</param>
		/// <param name="loopFileName">Full file path in resources for the loop of the music</param>
		public void PlayBGM(string startFileName, string loopFileName) {
			AudioClip start = null;
			AudioClip loop = null;

			if (startFileName != null) { 
				start = (AudioClip)Resources.Load(startFileName); 
			}

			if (loopFileName != null) { 
				loop  = (AudioClip)Resources.Load(loopFileName); 
			}

			PlayBGM(start, loop);
		}

		/// <summary>
		/// Play BGM from Audio Clip
		/// </summary>
		/// <param name="intro">Audio clip reference for the start of the music</param>
		/// <param name="loop">Audio clip reference for the loop of the music</param>
		public void PlayBGM(AudioClip intro, AudioClip loop) {
			if (loop == null) {
				return;
			}
			
			if (playingBGM) {
				StopBGM();
			}
			
			playingBGM = true;
			
			musicIntro.clip = intro;
			musicIntro.loop = false;
			
			musicLoop.clip = loop;
			musicLoop.loop = true;
			
			if (intro != null) {
				// Play start and queue loop
				musicIntro.Play();
				musicLoop.PlayDelayed(intro.length);
			} else {
				// play loop
				musicLoop.Play();
			}
		}

		/// <summary>
		/// To stop playing any background music
		/// </summary>
		public void StopBGM() {
			if (musicLoop != null) {
				musicLoop.Stop();
			}
			
			if (musicIntro != null) {
				musicIntro.Stop();
			}
			
			playingBGM = false;
		}

		//****************************************************
		// SOUND FX API
		//****************************************************

		/// <summary>
		/// Play sound effect from Resources
		/// </summary>
		/// <param name="sfx">Full file path in resources</param>
		public void PlaySFX(string sfx) {
			if (sfx == null) {
				return;
			}

			AudioClip audioClip = (AudioClip)Resources.Load(sfx);
			if (audioClip == null) {
				return;
			}

			audioSourceSFX[SFXCurrentCanal].clip = audioClip;
			audioSourceSFX[SFXCurrentCanal].Play();
			
			// select next available canal (audioSource) to play sound
			SFXCurrentCanal++;
			SFXCurrentCanal = SFXCurrentCanal % SFX_MAX_CANALS;
		}

		/// <summary>
		/// Play sound effect from Audio Clip
		/// </summary>
		/// <param name="audioClip">Audio clip reference</param>
		public void PlaySFX(AudioClip audioClip) {
			if (audioClip == null) {
				return;
			}
			
			audioSourceSFX[SFXCurrentCanal].clip = audioClip;
			audioSourceSFX[SFXCurrentCanal].Play();
			
			// select next available canal (audioSource) to play sound
			SFXCurrentCanal++;
			SFXCurrentCanal = SFXCurrentCanal % SFX_MAX_CANALS;
		}

		//****************************************************
		// VOLUME API
		//****************************************************

		/// <summary>
		/// Gets the current BGM volume.
		/// </summary>
		public float CurrentBGMVolume {
			get {
				if (musicIntro == null) {
					throw new NullReferenceException();
				}
				return musicIntro.volume;
			}
		}

		/// <summary>
		/// Gets the current SFX volume.
		/// </summary>
		public float CurrentSFXVolume {
			get {
				if (audioSourceSFX[0] == null) {
					throw new NullReferenceException();
				}
				return audioSourceSFX[0].volume;
			}
		}

		/// <summary>
		/// Toggle background music volume
		/// </summary>
		/// <param name="volume">On/Off</param>
		public void ToggleBGM(bool volume) {
			musicIntro.volume = volume ? backgroundVolume : 0;
			musicLoop.volume = volume ? backgroundVolume : 0;
		}
		
		/// <summary>
		/// Toggle sound effect volume
		/// </summary>
		/// <param name="volume">On/Off</param>
		public void ToggleSFX(bool volume) {
			for (int i = 0; i < SFX_MAX_CANALS; i++) {
				audioSourceSFX[i].volume = volume ? SFXVolume : 0;
			}
		}

		/// <summary>
		/// Set background music volume
		/// </summary>
		/// <param name="volume">Volume between 0 and 1</param>
		public void SetBGMVolume(float volume) {
			backgroundVolume = Mathf.Clamp01(volume);

			musicIntro.volume = backgroundVolume;
			musicLoop.volume = backgroundVolume;
		}
		
		/// <summary>
		/// Set sound effect volume
		/// </summary>
		/// <param name="volume">Volume between 0 and 1</param>
		public void SetSFXVolume(float volume) {
			SFXVolume = Mathf.Clamp01(volume);

			for (int i = 0; i < SFX_MAX_CANALS; i++) {
				audioSourceSFX[i].volume = SFXVolume;
			}
		}
	}
}
