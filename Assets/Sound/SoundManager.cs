using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
	[Header("General attributes")]
	public AnimationCurve fadeCurve;
	[Range(0.001f, 10f)] public float fadeTime = 10f;
	[Range(0f, 1f)] public float maxVolume = 1f;

    [Header("Playlist")]
    public AudioClip[] playlist;
    private AudioClip[] randomizedPlaylist; 

    [Header("Mixer")]
    public AudioMixerGroup mixer;

    private int curIdx = 0;
	private AudioClip inSound;
	private AudioSource mainSound;
	private AudioSource secondarySound;
	private bool fading;
	private float startedFadingAt;


	// Use this for initialization
	void Start () {
		mainSound = gameObject.AddComponent<AudioSource>();
		secondarySound = gameObject.AddComponent<AudioSource> ();

        mainSound.outputAudioMixerGroup = mixer;
        secondarySound.outputAudioMixerGroup = mixer;

		mainSound.Play ();
		secondarySound.Play ();

		fading = false;

		// checks every second if the mainSound is behaving as it should
		InvokeRepeating ("clampVolume", 0f, 1f);

        curIdx = Random.Range(0, playlist.Length - 1);
        PlayRandomMusicIndefinitely();

	}

    void RandomizeList(AudioClip[] list)
    {

    }
	
	// Update is called once per frame
	void Update () {
		Fade ();
	}

	private void Fade() {

		// if we are not currently fading
		if (!fading) {
			// if we get a fade request
			if (inSound != null) {
				startedFadingAt = Time.time; // save the time we started fading
				secondarySound.clip = inSound; // set the clip to secondary audio player
				secondarySound.time = 0f;
				secondarySound.Play ();
				inSound = null; // reset the in sound to null so we wont do it again
				fading = true;
			}

		} else {
			float curveTime = (Time.time - startedFadingAt) / fadeTime;
			float curveValue = fadeCurve.Evaluate (curveTime);

			mainSound.volume = (1f - curveValue) * maxVolume;
			secondarySound.volume = (curveValue) * maxVolume;

			// if we have finished our fading  
			if (curveTime >= 1f) {
				fading = false;

				// switch mainSound and secondarySound
				mainSound.clip = secondarySound.clip;
				mainSound.volume = maxVolume;
				mainSound.Play ();
				mainSound.time = secondarySound.time;

				secondarySound.clip = null;
				secondarySound.volume = 0f;
				secondarySound.time = 0f;
			}
		}
	}

	private void clampVolume() {
		mainSound.volume = Mathf.Min (maxVolume, mainSound.volume);
	}

	// Use this function from outside to fade another track in 
	public void MakeFade(AudioClip inMusic) {
		inSound = inMusic;
	}

    public void PlayRandomMusicIndefinitely()
    {
        // pick next
        AudioClip luckyOne = playlist[curIdx];
        curIdx = (curIdx + 1) % (playlist.Length - 1);

        this.MakeFade(luckyOne);

        Invoke("PlayRandomMusicIndefinitely", this.inSound.length - this.fadeTime);
    }
}

