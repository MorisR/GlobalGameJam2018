using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoviePlayer : MonoBehaviour {

    [SerializeField] MovieTexture movie;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        GetComponent<RawImage>().texture = movie;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = movie.audioClip;

        movie.Play();
        audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
