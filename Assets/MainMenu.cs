using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	Rect windowRect;
	
	// Use this for initialization
	void Start () {
		windowRect = new Rect(0, 0, Screen.width, Screen.height);
	}
	
	void OnGui() {
		GUI.Box(windowRect, "Main Menu");
		GUI.Button(new Rect(370, 350, Screen.width/4, Screen.height/24), "Suck it");
	}
}
