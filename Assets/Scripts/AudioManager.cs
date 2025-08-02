using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip clickSound;
	public AudioClip music;

	void Awake()
	{
		if(!!music && !!MusicConstants.music && MusicConstants.music.name == music.name)
		{
			Debug.Log("music matches");
			music = MusicConstants.music;
			GetComponent<AudioSource>().clip = music;

			GetComponent<AudioSource>().time = MusicConstants.time;
			
			GetComponent<AudioSource>().Play();
		}
		else
		{
			MusicConstants.music = music;
			GetComponent<AudioSource>().clip = music;
			GetComponent<AudioSource>().Play();
		}
		
	}

	void Update()
	{
		MusicConstants.time = GetComponent<AudioSource>().time;
	}

	public void ClickSound()
	{
		GetComponent<AudioSource>().PlayOneShot(clickSound);
	}
}

public static class MusicConstants
{
	public static AudioClip music;
	public static float time = 0f;
}
