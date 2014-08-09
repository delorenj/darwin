using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public GameState stateMachine;
	
	public static void PlayIntro() {
		SoundManager.PlaySong("Orbit01");
	}
}
