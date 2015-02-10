using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFreeFlight {

	/// <summary>
	/// Keep track of audio clips setup via setupSound(), and their corresponding
	/// AudioSoruce (which the Sound Manager keeps a private list).
	/// HACK1: Sound manager is setup on a component basis, which is very limmiting
	/// because ALL sounds come from the main game object only. I'd like to eventually
	/// push this off onto separate "sound" objects that are more modifyable. 
	/// HACK2: Sound manager is not well achitected for a *new* mechanic system. I think
	/// the mechanic system will come next, so I'm suspending extra features with this class
	/// until Mechanics are finished and strict requirements can be written.
	/// HACK3: Sounds will probably be managed by the various "mode" classes, meaning that's 
	/// where setupSound() will be called. That's bad because it will be less obvious when
	/// sounds are setup relative to starting the game, and there will probably be a performance
	/// impact if they're called on every mode change.  This will be fixed when mechanics get 
	/// properly refactored and Editors get written for them (which should be responsible for
	/// setting up sounds before the game starts). 
	/// </summary>
	[Serializable]
	public class SoundManager {

		private GameObject gameObject;
		public Dictionary<AudioClip, AudioSource> soundDict;

		public void init (GameObject go) {
			gameObject = go;
			soundDict = new Dictionary<AudioClip, AudioSource> ();
		}

		public void setupSound(AudioClip sound) {

			if (!sound)
				return;

			AudioSource source;
			if (!soundDict.TryGetValue (sound, out source)) {
				source = gameObject.AddComponent<AudioSource> ();
				soundDict.Add (sound, source);
			}

			source.loop = false;
			source.clip = sound;

		}

		public AudioSource getSource(AudioClip sound) {
			AudioSource source = null;
			soundDict.TryGetValue (sound, out source);
			return source;
		}

		public void playSound(AudioClip sound) {
			if (sound != null) {
				getSource (sound).Play ();
//				Debug.Log (string.Format ("Played sound: {0}", getSource(sound).name));
			} else {
				if (sound) {
					setupSound(sound);
					playSound(sound);
				}
			}
		}

	}

}
