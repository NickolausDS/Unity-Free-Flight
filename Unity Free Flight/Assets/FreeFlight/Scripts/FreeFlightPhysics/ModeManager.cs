using UnityEngine;
using System.Collections;
using UnityFreeFlight;

namespace UnityFreeFlight {


	public enum MovementModes {None, Ground, Flight};

	/// <summary>
	/// The mode manager keeps track of whether the flight object is on the ground or in the air, 
	/// and shifts control flow accordingly. Control flow in this case is the keyboard/mouse/joystick
	/// input groups, and which set of code is applied to each set of inputs. 
	/// on those inputs. 
	/// </summary>
	public class ModeManager {

		public BaseMode[] managers; 
		public FlightMode flightMode;
		public GroundMode groundMode;

		public ModeManager (GameObject go) {
			_activeMode = MovementModes.Flight;
			flightMode = new FlightMode (go);
			groundMode = new GroundMode (go);

			//Setup dynamic managers This needs to match the enum "movement Modes"
			managers = new BaseMode[3]; 
			managers [0] = null;
			managers [1] = groundMode;
			managers [2] = flightMode;
		}

		private MovementModes _activeMode;
		public MovementModes activeMode {
			get { return _activeMode; }
			set { 
				if (_activeMode != value) {
					currentMode.finishMode ();
					_activeMode = value;
					currentMode.startMode ();
				}
			}
		}

		public BaseMode currentMode {
			get { return managers[ (int) activeMode ]; }
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
