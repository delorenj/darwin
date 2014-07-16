using UnityEngine;

public class MainGUI : MonoBehaviour {

	public GUISkin skin;
	public int borderPadding;
	public Texture2D BtnLab;
	
	void OnGUI() {
		GUI.skin = skin;			
		
		if(GUI.Button(new Rect(Screen.width - borderPadding - 161, Screen.height - borderPadding - 161, 161, 161), BtnLab)) {
			Application.LoadLevel("MainMenu");
		}	
	}
}
