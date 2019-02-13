using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioClip music;
	public AudioSource source;

	void Start () {
		source.clip = music;
		source.Play();
	}
}
