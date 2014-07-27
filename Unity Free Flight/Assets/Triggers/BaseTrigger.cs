using UnityEngine;
using System.Collections;

public abstract class BaseTrigger : MonoBehaviour {

	public BaseTrigger chainToTrigger;
	public float delaySecondsToAction = .75f;
	public float delaySecsAfterAction = 2.0f; 

	private bool conditionSatisfied = false;
	private float conditionSatisfiedTime;

	private bool actionFired = false;
	private float actionSatisfiedTime;

	// Use this for initialization
	void Start () {
	
		initialAction ();

	}
	
	// Update is called once per frame
	void Update () {

		if (condition ()) {
			conditionSatisfied = true;
			conditionSatisfiedTime = Time.time;
		}

		if (conditionSatisfied && (Time.time > conditionSatisfiedTime + delaySecondsToAction)) {
			action ();

			actionFired = true;
			actionSatisfiedTime = Time.time;

			//We don't want to fire the action twice
			conditionSatisfied = false;
		}

		if (actionFired && (Time.time > actionSatisfiedTime + delaySecsAfterAction) ) {
			if (chainToTrigger)
				chainToTrigger.enabled = true;
			this.enabled = false;
		}
	
	}

	protected abstract void initialAction ();

	protected abstract bool condition ();

	protected abstract void action ();

}
