using UnityEngine;
using System.Collections;

public class FlappingSMB : StateMachineBehaviour {
	
	public delegate void FlapDelegate ();
	public FlapDelegate flap;
	
	
	// This will be called when the animator first transitions to this state.
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (flap != null) 
			flap();
	}
	
	
	// This will be called once the animator has transitioned out of the state.
	//		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}
	
	
	// This will be called every frame whilst in the state.
	//		override public void OnStateIK (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
}
