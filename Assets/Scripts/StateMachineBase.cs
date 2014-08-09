using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Reflection;

public abstract class StateMachineBase : MonoBehaviour {
	[HideInInspector]
	public Transform localTransform;
	[HideInInspector]
	public new Transform transform;
	[HideInInspector]
	public new Rigidbody rigidbody;
	[HideInInspector]
	public new Animation animation;
	[HideInInspector]
	public CharacterController controller;
	[HideInInspector]
	public new NetworkView networkView;
	[HideInInspector]
	public new Collider collider;
	[HideInInspector]
	public new GameObject gameObject;
	[HideInInspector]
	public StateMachineBase stateMachine;
	
	private float _timeEnteredState;
	
	public float timeInCurrentState
	{
		get
		{
			return Time.time - _timeEnteredState;
		}
	}
	
	
	void Awake()
	{
		state.executingStateMachine = stateMachine = this;
		localTransform = base.transform;
		Represent(base.gameObject);
		OnAwake();
	}
	
	void Represent(GameObject gameObjectToRepresent)
	{
		
		gameObject = gameObjectToRepresent;
		collider = gameObject.collider;
		transform = gameObject.transform;
		animation = gameObject.animation;
		rigidbody = gameObject.rigidbody;
		networkView = gameObject.networkView;
		controller = gameObject.GetComponent<CharacterController>();
		OnRepresent();
	}
	
	public void Refresh()
	{
		Represent(gameObject);
	}
	
	protected virtual void OnAwake()
	{
	}
	
	protected virtual void OnRepresent()
	{
	}
	
	static IEnumerator DoNothingCoroutine()
	{
		yield break;
	}
	
	static void DoNothing()
	{
	}
	
	static void DoNothingCollider(Collider other)
	{
	}
	
	static void DoNothingCollision(Collision other)
	{
	}
	
	
	public class State
	{
		
		public Action DoUpdate = DoNothing;
		public Action DoLateUpdate = DoNothing;
		public Action DoFixedUpdate = DoNothing;
		public Action<Collider> DoOnTriggerEnter = DoNothingCollider;
		public Action<Collider> DoOnTriggerStay = DoNothingCollider;
		public Action<Collider> DoOnTriggerExit = DoNothingCollider;
		public Action<Collision> DoOnCollisionEnter = DoNothingCollision;
		public Action<Collision> DoOnCollisionStay = DoNothingCollision;
		public Action<Collision> DoOnCollisionExit = DoNothingCollision;
		public Action DoOnMouseEnter = DoNothing;
		public Action DoOnMouseUp = DoNothing;
		public Action DoOnMouseDown = DoNothing;
		public Action DoOnMouseOver = DoNothing;
		public Action DoOnMouseExit = DoNothing;
		public Action DoOnMouseDrag = DoNothing;
		public Action DoOnGUI = DoNothing;
		public Func<IEnumerator> enterState = DoNothingCoroutine;
		public Func<IEnumerator> exitState = DoNothingCoroutine;
		
		public Enum currentState;
		public StateMachineBase executingStateMachine;
	}
	
	[HideInInspector]
	public State state = new State();
	
	public Enum currentState
	{
		get
		{
			return state.currentState;
		}
		set
		{
			if(stateMachine != this)
			{
				stateMachine.currentState = value;
			}
			else
			{
				
				if(state.currentState == value)
					return;
				
				ChangingState();
				state.currentState = value;
				state.executingStateMachine.state.currentState = value;
				ConfigureCurrentState();				
			}
		}
	}
	
	[HideInInspector]
	public Enum lastState;
	[HideInInspector]
	public StateMachineBase lastStateMachineBehaviour;
	
	public void SetState(Enum stateToActivate, StateMachineBase useStateMachine)
	{
		if(state.executingStateMachine == useStateMachine && stateToActivate == state.currentState)
			return;
		
		
		ChangingState();
		state.currentState = stateToActivate;
		state.executingStateMachine = useStateMachine;
		
		if(useStateMachine != this)
		{
			useStateMachine.stateMachine = this;
		} 
		state.executingStateMachine.state.currentState = stateToActivate;
		useStateMachine.Represent(gameObject);
		ConfigureCurrentState();
	}
	
	
	/// <summary>
	/// Caches previous states
	/// </summary>
	void ChangingState()
	{
		lastState = state.currentState;
		lastStateMachineBehaviour = state.executingStateMachine;
		_timeEnteredState = Time.time;
	}
	
	void ConfigureCurrentState()
	{
		if(state.exitState != null)
		{
			stateMachine.StartCoroutine(state.exitState());
		}
		
		//Now we need to configure all of the methods
		state.DoUpdate = state.executingStateMachine.ConfigureDelegate<Action>("Update", DoNothing);
		state.DoOnGUI = state.executingStateMachine.ConfigureDelegate<Action>("OnGUI", DoNothing);
		state.DoLateUpdate = state.executingStateMachine.ConfigureDelegate<Action>("LateUpdate", DoNothing);
		state.DoFixedUpdate = state.executingStateMachine.ConfigureDelegate<Action>("FixedUpdate", DoNothing);
		state.DoOnMouseUp = state.executingStateMachine.ConfigureDelegate<Action>("OnMouseUp", DoNothing);
		state.DoOnMouseDown = state.executingStateMachine.ConfigureDelegate<Action>("OnMouseDown", DoNothing);
		state.DoOnMouseEnter = state.executingStateMachine.ConfigureDelegate<Action>("OnMouseEnter", DoNothing);
		state.DoOnMouseExit = state.executingStateMachine.ConfigureDelegate<Action>("OnMouseExit", DoNothing);
		state.DoOnMouseDrag = state.executingStateMachine.ConfigureDelegate<Action>("OnMouseDrag", DoNothing);
		state.DoOnMouseOver = state.executingStateMachine.ConfigureDelegate<Action>("OnMouseOver", DoNothing);
		state.DoOnTriggerEnter = state.executingStateMachine.ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DoNothingCollider);
		state.DoOnTriggerExit = state.executingStateMachine.ConfigureDelegate<Action<Collider>>("OnTriggerExit", DoNothingCollider);
		state.DoOnTriggerStay = state.executingStateMachine.ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DoNothingCollider);
		state.DoOnCollisionEnter = state.executingStateMachine.ConfigureDelegate<Action<Collision>>("OnCollisionEnter", DoNothingCollision);
		state.DoOnCollisionExit = state.executingStateMachine.ConfigureDelegate<Action<Collision>>("OnCollisionExit", DoNothingCollision);
		state.DoOnCollisionStay = state.executingStateMachine.ConfigureDelegate<Action<Collision>>("OnCollisionStay", DoNothingCollision);
		state.enterState = state.executingStateMachine.ConfigureDelegate<Func<IEnumerator>>("EnterState", DoNothingCoroutine);
		state.exitState = state.executingStateMachine.ConfigureDelegate<Func<IEnumerator>>("ExitState", DoNothingCoroutine);
		
		EnableGUI();
		
		if(state.enterState != null)
		{
			stateMachine.StartCoroutine(state.enterState());
		}
		
		
	}
	
	Dictionary<Enum, Dictionary<string, Delegate>> _cache = new Dictionary<Enum, Dictionary<string, Delegate>>();
	
	T ConfigureDelegate<T>(string methodRoot, T Default) where T : class
	{
		
		Dictionary<string, Delegate> lookup;
		if(!_cache.TryGetValue(state.currentState, out lookup))
		{
			_cache[state.currentState] = lookup = new Dictionary<string, Delegate>();
		}
		Delegate returnValue;
		if(!lookup.TryGetValue(methodRoot, out returnValue))
		{
			
			var mtd = GetType().GetMethod(state.currentState.ToString() + "_" + methodRoot, System.Reflection.BindingFlags.Instance 
			                              | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod);
			
			if(mtd != null)
			{
				returnValue = Delegate.CreateDelegate(typeof(T), this, mtd);
			}
			else
			{
				returnValue = Default as Delegate;
			}
			lookup[methodRoot] = returnValue;
		}
		return returnValue as T;
		
	}
	
	
	
	protected void EnableGUI()
	{
		useGUILayout = state.DoOnGUI != DoNothing;
	}
	
	// Update is called once per frame
	void Update () 
	{
		state.DoUpdate();
	}
	
	void LateUpdate() 
	{
		state.DoLateUpdate();
	}
	
	void OnMouseEnter()
	{
		state.DoOnMouseEnter();
	}
	
	void OnMouseUp()
	{
		state.DoOnMouseUp();
	}
	
	void OnMouseDown()
	{
		state.DoOnMouseDown();
	}
	
	void OnMouseExit()
	{
		state.DoOnMouseExit();
	}
	
	void OnMouseDrag()
	{
		state.DoOnMouseDrag();
	}
	
	void FixedUpdate()
	{
		state.DoFixedUpdate();
	}
	void OnTriggerEnter(Collider other)
	{
		state.DoOnTriggerEnter(other);
	}
	void OnTriggerExit(Collider other)
	{
		state.DoOnTriggerExit(other);
	}
	void OnTriggerStay(Collider other)
	{
		state.DoOnTriggerStay(other);
	}
	void OnCollisionEnter(Collision other)
	{
		state.DoOnCollisionEnter(other);
	}
	void OnCollisionExit(Collision other)
	{
		state.DoOnCollisionExit(other);
	}
	void OnCollisionStay(Collision other)
	{
		state.DoOnCollisionStay(other);
	}
	void OnGUI()
	{
		state.DoOnGUI();
	}
	
	public IEnumerator WaitForAnimation(string name, float ratio)
	{
		var state = animation[name];
		return WaitForAnimation(state, ratio);
	}
	
	public static IEnumerator WaitForAnimation(AnimationState state, float ratio)
	{
		state.wrapMode = WrapMode.ClampForever;
		state.enabled = true;
		state.speed = state.speed == 0 ? 1 : state.speed;
		while(state.normalizedTime < ratio-float.Epsilon)
		{
			yield return null;
		}
	}
	
	public IEnumerator PlayAnimation(string name)
	{
		var state = animation[name];
		return PlayAnimation(state);
	}
	
	public static IEnumerator PlayAnimation(AnimationState state)
	{
		state.time = 0;
		state.weight = 1;
		state.speed = 1;
		state.enabled = true;
		var wait = WaitForAnimation(state, 1f);
		while(wait.MoveNext())
			yield return null;
		state.weight = 0;
		
	}
	
}
