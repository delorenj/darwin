using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : StateMachineBase {
	
	public Transform objectPrefab;
	
	public enum GameStates
	{
		Idle = 0,
		PlacingObject = 1
	}
	
	
	bool _busy;
	
	// Use this for initialization
	void Start () {
		currentState = GameStates.Idle;
	}
	
	bool hasPlacedObject;
	
	#region Idle
	
	void Idle_EnterState()
	{
		print ("Idle_EnterState...");	
	}
	
	void Idle_Update()
	{
		print ("Idle_Update()");
	}
	
	#endregion	
	
	#region PlacedObject
	
	void PlacingObject_EnterState()
	{
		print ("PlacingObject_EnterState()");
	}
	
	void PlacingObject_Update()
	{
		print ("PlacingObject_Update()");
	}
	
	#endregion
}
