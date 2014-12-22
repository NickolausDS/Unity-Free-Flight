

//==================
//Organization of this class is as follows:
// * Variable Declaration
// 		* Flight input Declaration
//		* Ground input Declaration
// * Unity Events
// * Ground Functionality
// * Takeoff/landing Functionality
// * Flight Functionality
// * Audio
//==================

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class FreeFlight : MonoBehaviour {

	/*
	 * None -- No action is taken by Free Flight
	 * Ground -- Get ground inputs from the player and apply them.
	 * Flight -- Get flight inputs from the player and apply them.
	 */ 
	public enum FlightState {None, Ground, Flight};
	private FlightState state = FlightState.Ground;
	public FlightState State {
		get { return state; }
		set { state = value; }
	}
	public bool applyFlightPhysicsOnGround = false;

	private CreatureFlightPhysics flightPhysics;
	public CreatureFlightPhysics FlightPhysics {
		get {
			if (flightPhysics == null) {
				flightPhysics = new CreatureFlightPhysics(rigidbody);
			}
			return flightPhysics;
		}
		set { flightPhysics = value;}
	}
	private FreeFlightAnimationHashIDs ffhash;
	private Animator anim;

	
	//=============
	//Unity Editor-Configurable Settings
	//=============

	public bool enabledGliding = true;
	//Basic gliding input, values in degrees
	public float maxTurnBank = 45.0f;
	public float maxPitch = 20.0f;
	public float directionalSensitivity = 2.0f;
	
	public bool enabledFlapping = true;
	public AudioClip flapSoundClip;
	private AudioSource flapSoundSource;
//	public float regularFlaptime = 0.5f;
//	public float minimumFlapTime = 0.2f;
	public float flapStrength = 60.0f;
//	public float downbeatStrength = 150.0f;
	
	public bool enabledFlaring = false;
	public AudioClip flareSoundClip;
	private AudioSource flareSoundSource;
	//The default pitch (x) we rotate to when we do a flare
	public float flareAngle = 70.0f;
	public float flareSpeed = 3.0f;
	
	public bool enabledDiving = false;
	public AudioClip divingSoundClip;
	private AudioSource divingSoundSource;

	public bool enabledTakeoff = true;
	public AudioClip takeoffSoundClip;
	private AudioSource takeoffSoundSource;

	public bool enabledLanding = true;
	public AudioClip landingSoundClip;
	private AudioSource landingSoundSource;
	//Max time "standUp" will take to execute.
	public float maxStandUpTime = 2.0f;
	//Speed which "standUp" will correct rotation. 
	public float standUpSpeed = 2.0f;

	public bool enabledCrashing = false;
	public float crashSpeed = 40f;
	public AudioClip crashSoundClip;
	private AudioSource crashSoundSource;

	public bool enabledWindNoise = true;
	public AudioClip windNoiseClip;
	private AudioSource windNoiseSource;
	public float windNoiseStartSpeed = 20.0f;
	public float windNoiseMaxSpeed = 200.0f;
	
	//===========
	//USER INPUT FLIGHT
	//===========
	

	//These protected vars are meant to be directly used or modified by the 
	//child class, and generally read from by the physics model. 
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
	[Range(-1.0f, 1.0f)]
	protected float _inputPitch = 0.0f;
	[Range(-1.0f, 1.0f)]
	protected float _inputBank = 0.0f;
	
	public bool InputTakeoff { get { return _inputTakeoff; } }
	public bool InputFlaring { get { return _inputFlaring; } }
	public bool InputDiving { get { return _inputDiving; } }
	public bool InputFlap { get { return _inputFlap; } }
	public float InputPitch { get { return _inputPitch; } }
	public float AnglePitch { get { return getPitch(_inputFlaring); } }
	public float InputBank { get { return _inputBank; } }
	public float AngleBank { get { return getBank (); } }
	
	public float launchTime = 0.2f;
	private float launchTimer;
	
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

	//===========
	//USER INPUT GROUND
	//===========

	public bool enabledGround = true;
	public AudioClip walkingNoiseClip;
	private AudioSource walkingNoiseSource;
	//meters/second
	public float maxGroundForwardSpeed = 4;
	//degrees/second
	public float groundDrag = 5;
	public float maxGroundTurningDegreesSecond = 20;
	//meters
	public bool enabledJumping = false;
	public float jumpHeight = .5f;
	public AudioClip jumpingNoiseClip;
	private AudioSource jumpingNoiseSource;

	[Range(-1.0f, 1.0f)]
	protected float _inputGroundForward;
	[Range(-1.0f, 1.0f)]
	protected float _inputGroundTurning;
	protected bool _inputJump;

	public float InputGroundForward { get { return _inputGroundForward; } }
	public float InputGroundTurning { get { return _inputGroundTurning; } }
	public bool InputJump { get { return _inputJump; } }


	//=============
	//Unity Events
	//=============
	
	void Awake() {
		flightPhysics = new CreatureFlightPhysics (rigidbody);
		setupSound (windNoiseClip, ref windNoiseSource);
		setupSound (flapSoundClip, ref flapSoundSource);
		setupSound (flareSoundClip, ref flareSoundSource);
		setupSound (divingSoundClip, ref divingSoundSource);
		setupSound (takeoffSoundClip, ref takeoffSoundSource);
		setupSound (landingSoundClip, ref landingSoundSource);
		setupSound (crashSoundClip, ref crashSoundSource);
		setupSound (walkingNoiseClip, ref walkingNoiseSource);
		setupSound (jumpingNoiseClip, ref jumpingNoiseSource);
		anim = GetComponentInChildren<Animator> ();
		ffhash = new FreeFlightAnimationHashIDs ();
	}
	
	void Start() {
		
	}
	
	/// <summary>
	/// Get input from the player 
	/// </summary>
	void Update() {
		if (isFlying() ) {
			getFlightInputs();
		} else if (isGrounded() ) {
			getGroundInputs();
		}
	}
	
	/// <summary>
	/// In relation to Update() this is where we decide how to act on the user input, then
	/// compute the physics and animation accordingly
	/// </summary>
	void FixedUpdate () {
		
		if (isFlying ()) {
			applyFlightInputs();
		} else if (applyFlightPhysicsOnGround && isGrounded ()) {
			flightPhysics.doStandardPhysics();
		}

		if (isGrounded () ) {
			applyGroundInputs();
		}

		if (enabledWindNoise)
			applyWindNoise ();
		
	}
	
	//Default behaviour when we hit an object (usually the ground) is to switch to a ground controller. 
	//Override in controller to change this behaviour.
	protected void OnCollisionEnter(Collision col) {
		land ();
	}
	
	//==================
	//Functionality -- Ground
	//==================

	public bool isGrounded() {
		if (state == FlightState.Ground)
			return true;
		return false;
	}

	private void getGroundInputs() {

		_inputGroundForward = Input.GetAxis("Vertical");
		_inputGroundTurning = Input.GetAxis ("Horizontal");
		_inputJump = Input.GetButton ("Jump");
		_inputTakeoff = _inputJump;

	}

	private void applyGroundInputs() {


		if (enabledJumping && _inputJump) {
			jump ();
		} else if(enabledGround) {
			groundMove();
		}


		if (enabledTakeoff)
			timedLaunch (_inputTakeoff);


	}

	private void jump() {
		anim.SetTrigger ("Jumping");
		playSound(jumpingNoiseSource);
		rigidbody.AddForce (0, jumpHeight, 0, ForceMode.Force);

	}

	private void groundMove() {
		rigidbody.drag = groundDrag;
		if (_inputGroundForward > 0f) {
			anim.SetBool (ffhash.walkingBool, true);
			rigidbody.AddRelativeForce (Vector3.forward * maxGroundForwardSpeed * _inputGroundForward * Time.deltaTime, ForceMode.VelocityChange);
		} else {
			anim.SetBool (ffhash.walkingBool, false);
		}
		rigidbody.rotation *= Quaternion.AngleAxis (maxGroundTurningDegreesSecond * _inputGroundTurning * Time.deltaTime, Vector3.up);
	}

	//==================
	//Functionality -- Takeoff/Landing
	//==================

	/// <summary>
	/// Sets _inputTakeoff to true if triggerSet stays true for X delay, set by 
	/// launch timer. The triggerSet is usually a button held down, like the jump
	/// button, but it can be anything. 
	/// </summary>
	/// <param name="triggerSet">If set to <c>true</c> for duration of launchTimer, triggers takeoff.</param>
	private void timedLaunch(bool triggerSet) {
		if (triggerSet == true) {
			if (launchTimer > launchTime)
				takeoff(true);
			else
				launchTimer += Time.deltaTime;
		} else {
			launchTimer = 0.0f;
		}
	}


	protected void takeoff(bool flapLaunch = false) {
		if (!isFlying()) {
			State = FlightState.Flight;
			anim.SetBool(ffhash.flyingBool, true);
			playSound (takeoffSoundSource);
			if(flapLaunch) 
				flap ();
		}
	}

	private void land() {
		if (isFlying()) {
			State = FlightState.Ground;
			anim.SetBool (ffhash.flaringBool, false);
			anim.SetBool(ffhash.flyingBool, false);
			if (enabledCrashing && flightPhysics.Speed >= crashSpeed) {
				anim.SetTrigger(ffhash.dyingTrigger);
				playSound (crashSoundSource);
			} else {
				playSound (landingSoundSource);
				StartCoroutine (standUp ());
			}
		}
	}

	/// <summary>
	/// Straightenes the flight object on landing, by rotating the roll and pitch
	/// to zero over time. Public vars "standUpSpeed" and "maxStandUpTime" can 
	/// be used to tweak behaviour.
	/// </summary>
	/// <returns>The up.</returns>
	protected IEnumerator standUp() {
		//Find the direction the flight object should stand, without any pitch and roll. 
		Quaternion desiredRotation = Quaternion.identity;
		desiredRotation.eulerAngles = new Vector3 (0.0f, transform.rotation.eulerAngles.y, 0.0f);
		//Grab the current time. We don't want 'standUp' to take longer than maxStandUpTime
		float time = Time.time;

		transform.rotation = desiredRotation; //Quaternion.Lerp (transform.rotation, desiredRotation, standUpSpeed * Time.deltaTime);

		//Break if the player started flying again, or if we've reached the desired rotation (within 5 degrees)
		while (!isFlying() && Quaternion.Angle(transform.rotation, desiredRotation) > 5.0f) {
			//Additionally break if we have gone over time
			if (time + maxStandUpTime < Time.time)
				break;
			//Correct the rotation
			transform.rotation = Quaternion.Lerp (transform.rotation, desiredRotation, standUpSpeed * Time.deltaTime);
			yield return null;
		}
		yield return null;
	}


	//==================
	//Functionality -- Flight
	//==================

	public bool isFlying() {
		if (state == FlightState.Flight)
			return true;
		return false;
	}

	private void getFlightInputs() { 
		_inputPitch = _inputInvertedSetting * -Input.GetAxis("Vertical");
		_inputBank = -Input.GetAxis ("Horizontal");
		
		if (enabledFlaring)
			_inputFlaring = Input.GetButton("WingFlare");
		
		//If the user presses down the jump button, flap
		_inputFlap = Input.GetButton("Jump"); 
		
		
		if (enabledDiving) {
			_inputDiving = Input.GetButton ("Dive");
			if (_inputDiving) {
				_inputLeftWingExposure = 0.0f;
				_inputRightWingExposure = 0.0f;
			} else {
				_inputRightWingExposure = 1.0f;
				_inputRightWingExposure = 1.0f;
			}
		}
	}
	
	void applyFlightInputs() {
		//HACK -- currently, drag is being fully calculated in flightPhysics.cs, so we don't want the
		//rigidbody adding any more drag. This should change, it's confusing to users when they look at
		//the rigidbody drag. 
		rigidbody.drag = 0.0f;
		//precedence is as follows: flaring, diving, regular gliding flight. This applies if the
		//player provides multiple inputs. Some mechanics can be performed at the same time, such 
		//as flapping while flaring, or turning while diving. 
		
		
		//Flaring takes precedence over everything
		if (enabledFlaring && _inputFlaring) {
			flare ();
			if(_inputFlap)
				flap ();
		} 
		
		//Diving takes precedence under flaring
		if(enabledDiving && _inputDiving && !_inputFlaring) {
			dive ();
		} else if (!_inputDiving && !flightPhysics.wingsOpen()) {
			//Simulates coming out of a dive
			dive ();
		}
		
		//Regular flight takes last precedence. Do regular flight if not flaring or diving.
		if ( !((enabledDiving && _inputDiving) || (enabledFlaring && _inputFlaring)) ) {
			flightPhysics.directionalInput(getBank (), getPitch (false), directionalSensitivity);
			//Allow flapping during normal flight
			if (_inputFlap)
				flap ();
		}

		if (!_inputFlaring)
			anim.SetBool (ffhash.flaringBool, false);
		if (!_inputDiving) {
			anim.SetBool (ffhash.divingBool, false);
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
			anim.SetBool (ffhash.flaringBool, true);
			//Flare is the same as directional input, except with exagerated pitch and custom speed. 
			flightPhysics.directionalInput(getBank (), getPitch (true), flareSpeed);
		}
	}
	
	protected void dive() {
		if (enabledDiving) {
			playSound (divingSoundSource);
			anim.SetBool (ffhash.divingBool, true);
			flightPhysics.wingFold(_inputLeftWingExposure, _inputRightWingExposure);
		}
		
	}

	//==================
	//Functionality -- Audio
	//==================

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
