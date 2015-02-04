using UnityEngine;
using UnityEngine.Internal;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityFreeFlight;

/// <summary>
/// Free Flight -- a Unity Component that adds flight to any unity object. 
/// </summary>
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Animator))]
public class FreeFlight : MonoBehaviour {

	ModeManager _modeManager; 
	public ModeManager modeManager {
		get { 
			if (_modeManager == null)
				_modeManager = new ModeManager(gameObject);
			return _modeManager; 
		}
		set { _modeManager = value;}
	}
 
	//=============
	//Unity Events
	//=============
	
//	void Awake() {
//		setupSound (windNoiseClip, ref windNoiseSource);
//		setupSound (flapSoundClip, ref flapSoundSource);
//		setupSound (flareSoundClip, ref flareSoundSource);
//		setupSound (divingSoundClip, ref divingSoundSource);
//		setupSound (takeoffSoundClip, ref takeoffSoundSource);
//		setupSound (landingSoundClip, ref landingSoundSource);
//		setupSound (crashSoundClip, ref crashSoundSource);
//		setupSound (walkingNoiseClip, ref walkingNoiseSource);
//		setupSound (jumpingNoiseClip, ref jumpingNoiseSource);
//		rigidbody.freezeRotation = true;
//		rigidbody.isKinematic = false;
//	}

	void SwitchModes(MovementModes newmode) {
		modeManager.switchModes (newmode);
	}
	
	/// <summary>
	/// Get input from the player 
	/// </summary>
	void Update() {
		modeManager.getInputs ();
	}
	
	/// <summary>
	/// In relation to Update() this is where we decide how to act on the user input, then
	/// compute the physics and animation accordingly
	/// </summary>
	void FixedUpdate () {	
		modeManager.applyInputs ();
	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		modeManager.switchModes (MovementModes.Ground);
	}


	//==================
	//Functionality -- Audio
	//==================

//	protected void applyWindNoise() {
//		
//		if (!windNoiseSource)
//			return;
//		
//		if (flightPhysics.Speed > windNoiseStartSpeed) {
//			
//			float volume = Mathf.Clamp (flightPhysics.Speed / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.0f, 1.0f);
//			windNoiseSource.volume = volume;
//			//We want pitch to pick up at about half the volume
//			windNoiseSource.pitch = Mathf.Clamp (0.9f + flightPhysics.Speed / 2.0f / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.9f, 1.5f);
//			//Use this to see how values are applied at various speeds.
//			//Debug.Log (string.Format ("Vol {0}, pitch {1}", audio.volume, audio.pitch));
//			if (! windNoiseSource.isPlaying) 
//				windNoiseSource.Play ();
//		} else {
//			windNoiseSource.Stop ();
//		}
//		
//	}
	
	/// <summary>
	/// Sets up the audio component for the sound source. Does nothing if the source
	/// already exists and has a clip. 
	/// </summary>
	/// <returns>A reference to the new audio source </returns>
	/// <param name="source">Source.</param>
	/// <param name="sound">Sound.</param>
//	protected AudioSource setupSound(AudioClip sound, ref AudioSource source) {
//		
//		if (!sound && source)
//			Destroy (source);
//		
//		if (!sound && !source)
//			return null;
//		
//		if (sound && !source) {
//			source = gameObject.AddComponent<AudioSource> ();
//			source.loop = false;
//		}
//		
//		if (!source.clip) {
//			source.clip = sound;
//		}
//		
//		return source;
//	}
//	
//	protected void playSound(AudioSource source) {
//		if (source) {
//			source.Play ();
//		}
//		
//	}

}
