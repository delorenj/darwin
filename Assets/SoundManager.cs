using UnityEngine;
using System.Collections;
using System.Linq;

public class SoundManager : Singleton<SoundManager> {

	public AudioClip[] Songs;
	public AudioSource SongSource;

	public static void PlaySong ()
	{
		Instance.playSong (0);
	}
	
	public static void PlaySong(string name) {
		Instance.playSong(name);
	}

	public static void PlaySong(int idx) {
		Instance.playSong(idx);
	}
	
// Privates
	private void playSong (int idx)
	{
		if(Enumerable.Range(0, Songs.Length-1).Contains(idx)) {
			SongSource.clip = Songs[idx];
			SongSource.Play ();			
		} else {
			throw new System.IndexOutOfRangeException("Song index is out of range");
		}
	}
	
	private void playSong(string name) {

		AudioClip found = null;
		
		foreach(AudioClip c in Songs) {
			if(c.name == name) {
				found = c;
			}
		}
		
		if(found) {
			SongSource.clip = found;
			SongSource.Play();
		} else {
			Debug.LogError("Song not found: " + name);
		}
	}

}
