using UnityEngine;
using System;
using System.Collections;
using UnityFreeFlight;

namespace UnityFreeFlight {


	public enum MovementModes {None, Ground, Flight};

	/// <summary>
	/// The mode manager keeps track of whether the flight object is on the ground or in the air, 
	/// and shifts control flow accordingly. Control flow in this case is the keyboard/mouse/joystick
	/// input groups, and which set of code is applied to each set of inputs. 
	/// on those inputs. 
	//TODO: Mode manager has no way to continue applying physics to non-current modes. 
	/// </summary>
	[Serializable]
	public class ModeManager {


		private BaseMode[] _managers; 
		public BaseMode[] managers {
			get { 
				//Setup dynamic managers This needs to match the enum "movement Modes"
				if (_managers == null) {
					_managers = new BaseMode[3]; 
					_managers [0] = null;
					_managers [1] = groundMode;
					_managers [2] = flightMode;
				}
				return _managers;
			}
			set {_managers = value;}
		}
		public FlightMode flightMode;
		public GroundMode groundMode;
		[SerializeField]
		public SoundManager soundManager;


		[SerializeField]
		private MovementModes _activeMode = MovementModes.Ground;
		public MovementModes activeMode {
			get { return _activeMode; }
			set { 
				if (_activeMode != value) {
					if (_activeMode != MovementModes.None)
						currentMode.finishMode ();
					_activeMode = value;
					if (_activeMode != MovementModes.None)
						currentMode.startMode ();
				}
			}
		}

		public void init (GameObject go) {
			if (soundManager == null)
				soundManager = new SoundManager();
			soundManager.init (go);

			if (flightMode == null)
				flightMode = new FlightMode ();
			flightMode.init (go, soundManager);

			if (groundMode == null)
				groundMode = new GroundMode ();
			groundMode.init (go, soundManager);

//			Debug.Log (string.Format ("groundmode {0}, flightmode {1}", groundMode, flightMode));
		

		}

		public void start () {
//			Debug.Log ("active mode: " + currentMode);
			if (activeMode != MovementModes.None)
				currentMode.startMode ();
		}

		public BaseMode currentMode {
			get {return managers[ (int) activeMode ]; }
		}

		public void getInputs() {
			if (activeMode != MovementModes.None)
				currentMode.getInputs ();
		}

		public void applyInputs() {
			if (activeMode != MovementModes.None)
				currentMode.applyInputs ();
		}

		public bool isGrounded() {
			if (activeMode == MovementModes.Ground)
				return true;
			return false;
		}

		public bool isFlying() {
			if (activeMode == MovementModes.Flight)
				return true;
			return false;
		}

		public void switchModes(MovementModes newmode) {
			activeMode = newmode;
		}
	}
}
