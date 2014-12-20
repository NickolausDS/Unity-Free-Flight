using UnityEngine;
using System.Collections;

/// <summary>
/// Hash IDs for animation. These numbers will be gotten from the animator on startup. The reason it isn't a 
/// MonoBehaviour is to avoid additional clutter on the editor screen. 
/// </summary>
public class FreeFlightAnimationHashIDs {

	public int speedFloat;
	public int angularSpeedFloat;
	public int flyingBool;
	public int glidingState;
	public int flappingTrigger;
	public int flappingState;
	public int flaringBool;
	public int dyingTrigger;
	public int walkingBool;
	

	public FreeFlightAnimationHashIDs () {
		speedFloat = Animator.StringToHash("Speed");
		angularSpeedFloat = Animator.StringToHash("AngularSpeed");
		flyingBool = Animator.StringToHash ("Flying");
		glidingState = Animator.StringToHash ("Base Layer.Gliding");
		flappingTrigger = Animator.StringToHash ("Flapping");
		flappingState = Animator.StringToHash ("Base Layer.Flapping");
		flaringBool = Animator.StringToHash ("Flaring");
		dyingTrigger = Animator.StringToHash ("Dying");
		walkingBool = Animator.StringToHash ("Walking");
	}
}
