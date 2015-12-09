using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour 
{
	public enum SoundFX
	{
		Good,
		Bad,
		Construct,
	}

	public enum Music
	{
		Menu,
		Game,
	}

	#region musics
	public AudioClip menuTheme;
	public AudioClip gameTheme;
	#endregion

	#region Sound FX
	public AudioClip good1;
	public AudioClip good2;
	public AudioClip bad1;
	public AudioClip bad2;
	public AudioClip construct;
	#endregion

	#region singleton
	private static SoundController instance;
	public static SoundController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<SoundController>();

			return instance;
		}
	}
	#endregion

	public AudioSource musicSource;
	public AudioSource effectsSource;

	// Use this for initialization
	void Start () 
	{

	}

	public static void PlaySoundFX(SoundFX sound)
	{
		AudioClip clip = Instance.GetAudioClip(sound);
		Instance.effectsSource.PlayOneShot(clip);
	}

	public static void PlayMusic(Music music)
	{
		AudioClip clip = Instance.GetAudioClip(music);
		Instance.musicSource.clip = clip;
		Instance.musicSource.Stop();
		Instance.musicSource.Play();
	}

	private AudioClip GetAudioClip(SoundFX type)
	{
		AudioClip clip = null;
		float rnd;

		switch(type)
		{
			case SoundFX.Good:
				rnd = Random.Range(0, 1f);
				if(rnd < 0.5f)
					clip = good1;
				else
					clip = good2;
				break;
				
			case SoundFX.Bad:
				rnd = Random.Range(0, 1f);
				if(rnd < 0.5f)
					clip = bad1;
				else
					clip = bad2;
				break;

			case SoundFX.Construct:
				clip = construct;
				break;
		}
		
		return clip;
	}

	private AudioClip GetAudioClip(Music type)
	{
		AudioClip clip = null;

		switch(type)
		{
			case Music.Game:
				clip = gameTheme;
				break;

			case Music.Menu:
				clip = menuTheme;
				break;
		}

		return clip;
	}
}
