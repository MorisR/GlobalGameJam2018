using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoviePlayer : MonoBehaviour {

    [SerializeField] MovieTexture movie;
    [SerializeField] string sceneToLoad;
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

        if (!movie.isPlaying && sceneToLoad != "")
            LevelManager.Instance.LoadScene(sceneToLoad);
		
	}
}
