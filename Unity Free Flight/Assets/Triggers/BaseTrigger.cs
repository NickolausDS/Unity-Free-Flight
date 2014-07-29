using UnityEngine;
using System.Collections;
/// <summary>
/// Triggers are used to provide dynamic gameplay to the player
/// whenever they do something important. When the 'condition'
/// for the trigger is satisfied, the 'action' happens. The 
/// conditions and actions can pretty much be anything, see
/// the various trigger classes (specifically the baseTrigger
/// does nothing, and is used by all the child trigger classes). 
/// 
/// </summary>
public abstract class BaseTrigger : MonoBehaviour {
	/* Implementing triggers:
	 * 
	 * You need to implement the following for triggers
	 * to work:
	 * 	initialAction(), condition(), and action()
	 * 
	 * The delays provide an organic way to fire triggers.
	 * delaySecsToCondition -- delays before trigger is even active (in seconds)
	 * delaySecstoAction -- buffer time after the condition, but before the action fires
	 * delaySecsAfterAction -- delay time (in seconds) after action has fired, and the script
	 * is disabled (and the next trigger activated, if it exists). 
	 */


	public BaseTrigger chainToTrigger;
	public float delaySecsToCondition = 0.0f;
	public float delaySecsToAction = 2.0f;
	public float delaySecsAfterAction = 0.0f; 

	private bool preConditionDelayDone = false;
	private float preConditionTime;


	private bool conditionSatisfied = false;
	private float conditionSatisfiedTime;

	private bool actionFired = false;
	private float actionSatisfiedTime;

	// Use this for initialization
	void Start () {

		preConditionTime = Time.time;

	}
	
	// Update is called once per frame
	void Update () {

		//Wait for a while before we do anything
		if (!preConditionDelayDone) {
			if (Time.time > delaySecsToCondition + preConditionTime) {
				preConditionDelayDone = true;
				initialAction();
			} else {
				return;
			}
		}

		//Check the condition
		if (!conditionSatisfied && condition ()) {
			conditionSatisfied = true;
			conditionSatisfiedTime = Time.time;
		}

		//Check the action
		if (!actionFired && conditionSatisfied && (Time.time > conditionSatisfiedTime + delaySecsToAction)) {
			action ();

			actionFired = true;
			actionSatisfiedTime = Time.time;

			//We don't want to fire the action twice
			//conditionSatisfied = false;
		}

		//Fire the action, and then 'chain' another trigger active if there is one
		if (actionFired && (Time.time > actionSatisfiedTime + delaySecsAfterAction) ) {
			if (chainToTrigger)
				chainToTrigger.enabled = true;
			reset ();
			this.enabled = false;
		}
	
	}

	/// <summary>
	/// Reset the Trigger so it can be used again. Note, that Start() needs to be
	/// called seperate from this method, which is done automatically if this script
	/// is started with: enabled=true
	/// </summary>
	public void reset () {
		preConditionDelayDone = false;
		conditionSatisfied = false;
		actionFired = false;
	}

	/// <summary>
	/// Any initialization you want to happen *before* we start checking
	/// the condition. Initialization waits for delaySecsToCondition before
	/// calling this method.
	/// </summary>
	protected abstract void initialAction ();

	/// <summary>
	/// The condition is called every frame update until it returns true.
	/// We wait for delaySecsToAction before firing the action, after this
	/// method returns true.
	/// </summary>
	protected abstract bool condition ();

	/// <summary>
	/// Called when the condition returns true. Afterward, the Trigger
	/// is disabled, and the next Chain trigger enabled (if it exists).
	/// delaySecsAfterAction waits before terminating the trigger. 
	/// </summary>
	protected abstract void action ();

}
