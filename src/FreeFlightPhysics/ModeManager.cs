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
	//TODO 2: (Oct 2015 ~pre-v0.5.0) Mode Manager defines flight mode and ground mode explicitly. This
	//makes the code below rather dirty, as ONLY the two modes below can ever exist. "MovementModes" enum is 
	//another dirty solution -- it cements the two modes and requires a recompile for adding more. One 
	//of two solutions should be used: Modes should be loaded dynamically with the Polymorphic Serializer ( 
	//as is done with mechanics), or be loaded with ScriptableObjects (which is how Unity defines the _proper_
	//methodology for loading dynamic scripts). 
	//TODO 3: As mentioned in the point above, the "MovementModes" enum is a terrible solution. It was an early hack
	//to get modes working, and needs to be updated. It prevents dynamic additions/removals of modes without changes to
	//the code below. A better solution would be to mirror the Mechanics Editor, which gathers mode information by applying 
	//reflection on the current list of dynamic modes (once dynamic modes are also implemented).
	/// </summary>
	[Serializable]
	public class ModeManager {


		public BaseMode[] managers;

		public FlightMode flightMode;
		public GroundMode groundMode;

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

			if (flightMode == null)
				flightMode = new FlightMode ();
			flightMode.init (go);

			if (groundMode == null)
				groundMode = new GroundMode ();
			groundMode.init (go);

			managers = new BaseMode[3]; 
			managers [0] = null;
			managers [1] = groundMode;
			managers [2] = flightMode;

		}

		public void start () {
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
