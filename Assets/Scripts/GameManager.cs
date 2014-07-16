using UnityEngine;

public class GameManager : ScriptableObject {

	private AudioSource SoundSource;
	private static GameManager instance;

	public static GameManager Instance {
		get {
			if(instance == null) {
				instance = ScriptableObject.CreateInstance<GameManager>();
			}	
			return instance;
		}
	}
	
	public void PlaySong() {
		GameObject go = GameObject.Find("SoundEmitter");
		SoundSource = go.GetComponent<AudioSource>();
		
		if(!SoundSource.isPlaying) {
			Debug.Log ("Setting clip...");
			SoundSource.Play();		
		}
		
		Debug.Log ("Playing: " + SoundSource.isPlaying);
		Debug.Log ("Name: " + SoundSource.clip.name);
	}

	// Privates	
	private GameManager() {

	}
	
	
}
