using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFreeFlight {

	/// <summary>
	/// Sound manager.
	/// </summary>
	[Serializable]
	public class SoundManager {

		private static System.Random randomNumber = new System.Random ();


		private GameObject _gameObject;
		public GameObject gameObject {
			get { return _gameObject;}
			set { init (value); }
		}

		[NonSerialized]
		public AudioSource audioSource;

		/// <summary>
		/// Initialize the sound manager to use the specified game object with an attached
		/// Audio Source.
		/// </summary>
		/// <param name="go">Go.</param>
		public void init (GameObject go) {
			_gameObject = go;
			audioSource = gameObject.GetComponent<AudioSource> ();
		}

		/// <summary>
		/// Play an audio clip, with an optional delay. 
		/// </summary>
		/// <param name="sound">Sound.</param>
		/// <param name="delay">Delay.</param>
		public void playSound(AudioClip sound, ulong delay = 0) {
			if (sound != null) {
				if (audioSource != null) {
					audioSource.clip = sound;
					audioSource.Play(delay);
				} else {
					//Try one last time to setup the audio source. Since it's fairly common for developers
					//to change things during runtime, this makes things very convienient for them. 
					audioSource = _gameObject.GetComponent<AudioSource> ();
					if (audioSource == null)
						Debug.LogError (string.Format ("No audio source setup for '{0}', unable to play sounds.", gameObject.name));
				}
			}
		}

		/// <summary>
		/// Play a random sound from an array. 
		/// </summary>
		/// <param name="sounds">Sounds.</param>
		/// <param name="delay">Delay.</param>
		public void playRandomSound(AudioClip[] sounds, ulong delay=0) {
			if (sounds != null)
				playSound(sounds[randomNumber.Next (sounds.Length)], delay);
		}



	}

}
