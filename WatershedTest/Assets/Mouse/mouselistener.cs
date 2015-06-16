using UnityEngine;
using System.Collections;
//using System.Linq;
using System;
using System.Collections.Generic;

public class mouselistener : MonoBehaviour {

	public enum mouseState{OS=0,TERRAIN=1};
	public mouseState state;
	public mouseState[] states;
	public Vector2 mousePos
	{
	   get{return Input.mousePosition;}
	}
	
	public mouseState State
	{
	   get{return state;}
	   set{state = value;}
	}
	
	// Use this for initialization
	void Start () {
		state = mouseState.TERRAIN;
		var temp = new ArrayList(Enum.GetValues(typeof(mouseState)));
		states  = new mouseState[temp.Count];
		foreach (var i in temp)
		{
			states[temp.IndexOf(i)] = (mouseState)i;
		}
        
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Y))
		{
			state = states[(Array.IndexOf<mouseState>(states,state)+1)%states.Length];
			Debug.Log(state);
			//state =  Enum.GetValues(typeof(mouseState))[% Enum.GetNames(typeof(mouseState)).Length];
		}
	    Cursor.visible = state == mouseState.OS;
	}
}
