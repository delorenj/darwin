using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public static void PlayIntro() {
		SoundManager.PlaySong("Orbit01");
	}
}
