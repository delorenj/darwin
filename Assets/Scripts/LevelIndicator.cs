using UnityEngine;

public class LevelIndicator : MonoBehaviourBase {

	public GUISkin skin;
	public int borderPadding;
	public Texture2D bar;
	public Texture2D filling;
	
	void OnGUI() {
		GUI.skin = skin;
		GUI.BeginGroup(new Rect(borderPadding, borderPadding, 381, 92));
			GUI.DrawTexture(new Rect(0, 0, bar.width/2, bar.height/2), filling);
			GUI.BeginGroup(new Rect(0, 0, bar.width, bar.height));
				GUI.DrawTexture(new Rect(0, 0, bar.width, bar.height), bar);
			GUI.EndGroup();
		GUI.EndGroup();
	}
}
