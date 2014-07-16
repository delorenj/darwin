using UnityEngine;
using System.Collections;

public class MusicLoop : MonoBehaviour {

	public AudioClip SoundClip;
	private AudioSource SoundSource;
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		SoundSource = gameObject.AddComponent<AudioSource>();
		//SoundSource.playOnAwake = false;
		//SoundSource.rolloffMode = AudioRolloffMode.Logarithmic;
		SoundSource.loop = true;
	}
	
	void Start()
	{
		SoundSource.clip = SoundClip;
		SoundSource.Play(); 
	}
}
