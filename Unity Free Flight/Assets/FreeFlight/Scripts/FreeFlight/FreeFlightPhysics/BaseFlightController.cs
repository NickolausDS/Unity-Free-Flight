/*
 * Base Flight Controller
 * 
 * Note: This is an incomplete controller! It should never be added directly to a
 * gameobject! 
 * 
 * Inherit from this class if you want to write your own controller. Lots of effort was
 * put in to decouple the mechanic from the controller. The functionality is here, while
 * the feel for how it works is left up to the controller. If you find yourself writing
 * complicated controllers, check here to make sure the functionality you need doesn't 
 * already exist here.
 * 
 */


using UnityEngine;
using System.Collections;

[RequireComponent (typeof(FreeFlight))]
public class BaseFlightController : MonoBehaviour {

	public FlightMechanics flightPhysics;
	/// <summary>
	/// true if flying, false if on the ground.
	/// </summary>
	public bool isFlying = true;

	//=============
	//Unity Editor-Configurable Settings
	//=============

	//Basic gliding input, values in degrees
	public float maxTurnBank = 45.0f;
	public float maxPitch = 20.0f;
	public float directionalSensitivity = 2.0f;

	public bool enabledFlapping = true;
	public AudioClip flapSoundClip;
	private AudioSource flapSoundSource;
	public float regularFlaptime = 0.5f;
	public float minimumFlapTime = 0.2f;
	public float flapStrength = 600.0f;
	public float downbeatStrength = 150.0f;

	public bool enabledFlaring = false;
	public AudioClip flareSoundClip;
	private AudioSource flareSoundSource;
	//The default pitch (x) we rotate to when we do a flare
	public float flareAngle = 70.0f;
	public float flareSpeed = 3.0f;

	public bool enabledDiving = false;
	public AudioClip divingSoundClip;
	private AudioSource divingSoundSource;

	public bool crashingEnabled = false;
	public float crashSpeed = 40f;

	public AudioClip takeoffSoundClip;
	private AudioSource takeoffSoundSource;

	public AudioClip landingSoundClip;
	private AudioSource landingSoundSource;

	public AudioClip crashSoundClip;
	private AudioSource crashSoundSource;

	//Max time "standUp" will take to execute.
	public float maxStandUpTime = 2.0f;
	//Speed which "standUp" will correct rotation. 
	public float standUpSpeed = 2.0f;

	public AudioClip windNoiseClip;
	private AudioSource windNoiseSource;
	public float windNoiseStartSpeed = 20.0f;
	public float windNoiseMaxSpeed = 200.0f;

	public FreeFlightAnimationHashIDs ffhash;
	public Animator anim;

	//===========
	//USER INPUT
	//===========

	//These protected vars are meant to be directly used or modified by the 
	//child class, and generally read from by the physics model. 
	protected Quaternion _inputDirection;
	[Range(0.0f, 1.0f)]
	protected float _inputLeftWingExposure = 1.0f;
	[Range(0.0f, 1.0f)]
	protected float _inputRightWingExposure = 1.0f;
	public float LeftWingInput { get { return _inputLeftWingExposure; } }
	public float RightWingInput { get { return _inputRightWingExposure; } }	
	public float LeftWingExposure { get { return flightPhysics.LeftWingExposure; } }
	public float RightWingExposure { get { return flightPhysics.RightWingExposure; } }
	protected int _inputInvertedSetting = -1;
	protected bool _inputTakeoff = false;
	protected bool _inputFlaring = false;
	protected bool _inputDiving = false;
	protected bool _inputFlap = false;
	protected bool _inputSprint = false;
	[Range(-1.0f, 1.0f)]
	protected float _inputPitch = 0.0f;
	[Range(-1.0f, 1.0f)]
	protected float _inputBank = 0.0f;

	public bool InputTakeoff { get { return _inputTakeoff; } }
	public bool InputFlaring { get { return _inputFlaring; } }
	public bool InputDiving { get { return _inputDiving; } }
	public bool InputFlap { get { return _inputFlap; } }
	public bool InputSprint { get { return _inputSprint; } }
	public float InputPitch { get { return _inputPitch; } }
	public float AnglePitch { get { return getPitch(_inputFlaring); } }
	public float InputBank { get { return _inputBank; } }
	public float AngleBank { get { return getBank (); } }



	//Even though Inverted as a property here is invisible to the inspector, 
	//using the property in this way makes it convienient to access externally,
	//in order to *toggle* the setting on and off. Expressing _invertedSetting internally
	//an integer makes it very easy to apply to input. 
	public bool Inverted {
		get {
			if (_inputInvertedSetting == 1) 
				return true; 
			return false;
		}
		set {
			if (value == true) 
				_inputInvertedSetting = -1;
			else
				_inputInvertedSetting = 1;
		}
	}

	//===================
	//Stateful Variables
	//===================

	protected bool isFlapping;
	/// <summary>
	/// True when flapping, false otherwise.
	/// </summary>
	public bool IsFlapping { get { return isFlapping; } }

	//=============
	//Unity Events
	//=============

	void Awake() {
		flightPhysics = new FlightMechanics (rigidbody);
		setupSound (windNoiseClip, ref windNoiseSource);
		setupSound (flapSoundClip, ref flapSoundSource);
		setupSound (flareSoundClip, ref flareSoundSource);
		setupSound (divingSoundClip, ref divingSoundSource);
		setupSound (takeoffSoundClip, ref takeoffSoundSource);
		setupSound (landingSoundClip, ref landingSoundSource);
		setupSound (crashSoundClip, ref crashSoundSource);
		anim = GetComponentInChildren<Animator> ();
		ffhash = new FreeFlightAnimationHashIDs ();
	}

	void Start() {

	}

	/// <summary>
	/// This is where all player input control code goes, once this class is overridden. 
	/// </summary>
	void Update() {
		Debug.LogWarning ("Base Flight Controller is not a valid controller! Please add a different flight controller," +
			"or inherit from this class if you intend to write your own. Offending Game Object: " + gameObject.name);
		this.enabled = false;
	}

	/// <summary>
	/// In relation to Update() this is where we decide how to act on the user input, then
    /// compute the physics and animation accordingly
	/// </summary>
	void FixedUpdate () {

		if (_inputTakeoff && !isFlying)
			takeoff();

		if (isFlying) {
			applyStandardFlightMechanics();
		}

		applyWindNoise ();

	}

	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		if (isFlying) {
			isFlying = false;
			anim.SetBool(ffhash.flyingBool, false);
			if (crashingEnabled && flightPhysics.Speed >= crashSpeed) {
				anim.SetTrigger(ffhash.dyingTrigger);
				playSound (crashSoundSource);
			} else {
				playSound (landingSoundSource);
				StartCoroutine (standUp ());
			}
		}
	}

	//==================
	//Functionality
	//==================


	void applyStandardFlightMechanics() {
		//precedence is as follows: flaring, diving, regular gliding flight. This applies if the
		//player provides multiple inputs. Some mechanics can be performed at the same time, such 
		//as flapping while flaring, or turning while diving. 


		//Flaring takes precedence over everything
		if (_inputFlaring) {
			flare ();
			if(_inputFlap)
				flap ();
		} 

		//Diving takes precedence under flaring
		if(_inputDiving && !_inputFlaring) {
			dive ();
		} else if (!_inputDiving && !flightPhysics.wingsOpen()) {
			//Simulates coming out of a dive
			dive ();
		}

		//Regular flight takes last precedence
		if(!_inputDiving && !_inputFlaring) {
			flightPhysics.directionalInput(getBank (), getPitch (false), directionalSensitivity);
			//Allow flapping during normal flight
			if (_inputFlap)
				flap ();
		}
		
		flightPhysics.doStandardPhysics ();
			
	}

	/// <summary>
	/// Calculates pitch, based on user input and configured pitch parameters.
	/// </summary>
	/// <returns>The pitch in degrees.</returns>
	/// <param name="flare">If set to <c>true</c> calculates pitch of a flare angle.</param>
	protected float getPitch(bool flare) {
		if (flare)
			return _inputPitch * maxPitch - flareAngle;
		else
			return _inputPitch * maxPitch;
	}

	protected float getBank() {
		return _inputBank * maxTurnBank;
	}

	protected void takeoff(bool flapLaunch = false) {
		if (!isFlying) {

			//It sometimes takes a few milliseconds for this to switch over. We don't want to do any phisics in the meantime.
			//This will no longer be required after character controllers go away. 
			//if (rigidbody.isKinematic)
			//	rigidbody.isKinematic = false;
			isFlying = true;
			anim.SetBool(ffhash.flyingBool, true);
			playSound (takeoffSoundSource);
			if(flapLaunch) 
				flap ();
		}
	}

	protected void flap() {
		if(!enabledFlapping) {
			return;
		}
		AnimatorStateInfo curstate = anim.GetCurrentAnimatorStateInfo (0);
		if (curstate.nameHash != ffhash.flappingState) {
			playSound (flapSoundSource);
			rigidbody.AddForce (rigidbody.rotation * Vector3.up * flapStrength);
			anim.SetTrigger (ffhash.flappingTrigger);
		}
	}

	protected void flare() {
		if (enabledFlaring) {
			playSound (flareSoundSource);
			//Flare is the same as directional input, except with exagerated pitch and custom speed. 
			flightPhysics.directionalInput(getBank (), getPitch (true), flareSpeed);
		}
	}

	protected void dive() {
		if (enabledDiving) {
			playSound (divingSoundSource);
			flightPhysics.wingFold(_inputLeftWingExposure, _inputRightWingExposure);
		}

	}

	/// <summary>
	/// Straightenes the flight object on landing, by rotating the roll and pitch
	/// to zero over time. Public vars "standUpSpeed" and "maxStandUpTime" can 
	/// be used to tweak behaviour.
	/// </summary>
	/// <returns>The up.</returns>
	protected IEnumerator standUp() {
		
		//Make sure no physics are executed while this happens. 
		//enableNoneMode = true;
		//Dis-allow physics, which prevents 'falling over' before the object can stand
		rigidbody.isKinematic = true;
		//Find the direction the flight object should stand, without any pitch and roll. 
		Quaternion desiredRotation = Quaternion.identity;
		desiredRotation.eulerAngles = new Vector3 (0.0f, transform.rotation.eulerAngles.y, 0.0f);
		//Grab the current time. We don't want 'standUp' to take longer than maxStandUpTime
		float time = Time.time;
		
		//Break if the player started flying again, or if we've reached the desired rotation (within 5 degrees)
		while (!isFlying && Quaternion.Angle(transform.rotation, desiredRotation) > 5.0f) {
			//Additionally break if we have gone over time
			if (time + maxStandUpTime < Time.time)
				break;
			//Correct the rotation
			transform.rotation = Quaternion.Lerp (transform.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
			yield return null;
		}
		
		//enableGroundMode = true;
		
	}

	protected void applyWindNoise() {

		if (!windNoiseSource)
			return;

		if (flightPhysics.Speed > windNoiseStartSpeed) {
			
			float volume = Mathf.Clamp (flightPhysics.Speed / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.0f, 1.0f);
			windNoiseSource.volume = volume;
			//We want pitch to pick up at about half the volume
			windNoiseSource.pitch = Mathf.Clamp (0.9f + flightPhysics.Speed / 2.0f / (windNoiseStartSpeed + windNoiseMaxSpeed), 0.9f, 1.5f);
			//Use this to see how values are applied at various speeds.
			//Debug.Log (string.Format ("Vol {0}, pitch {1}", audio.volume, audio.pitch));
			if (! windNoiseSource.isPlaying) 
				windNoiseSource.Play ();
		} else {
			windNoiseSource.Stop ();
		}

	}

	/// <summary>
	/// Sets up the audio component for the sound source. Does nothing if the source
	/// already exists and has a clip. 
	/// </summary>
	/// <returns>A reference to the new audio source </returns>
	/// <param name="source">Source.</param>
	/// <param name="sound">Sound.</param>
	protected AudioSource setupSound(AudioClip sound, ref AudioSource source) {

		if (!sound && source)
			Destroy (source);

		if (!sound && !source)
			return null;
		
		if (sound && !source) {
			source = gameObject.AddComponent<AudioSource> ();
			source.loop = false;
		}
		
		if (!source.clip) {
			source.clip = sound;
		}
		
		return source;
	}

	protected void playSound(AudioSource source) {
		if (source) {
			source.Play ();
		}

	}


}
