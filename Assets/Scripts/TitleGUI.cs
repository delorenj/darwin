using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {

	public GUISkin skin;
	
	int buttonHeight = 50;
	int buttonWidth = 150;
	float halfScreenWidth;
	float halfButtonWidth;
	
	void Start() {
		halfScreenWidth = Screen.width/2;
		halfButtonWidth = buttonWidth/2;
		GameManager.Instance.PlaySong();
	}
	
	void OnGUI() {
		GUI.skin = skin;
		
		if(GUI.Button(new Rect(halfScreenWidth-halfButtonWidth,800,buttonWidth,buttonHeight), "PLAY")) {
			Application.LoadLevel("Forest");
		}
	}
}
