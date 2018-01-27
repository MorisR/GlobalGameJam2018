using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {
  
    [SerializeField] bool isScrolling;
    [SerializeField] float scrollSpeed;
    [SerializeField] Vector2 dircrtion;
    [SerializeField] SpriteRenderer _renderer;

    public void startScroll()
    {
        isScrolling = true; 
    }

    public void stopScroll()
    {
        isScrolling = false;

    }

    // Use this for initialization
    void Start () {
      
      //  dircrtion = dircrtion.normalized;

    }
	
	// Update is called once per frame
	void Update () {

        if (isScrolling)
        {
            Vector2 offset = dircrtion * Time.time * scrollSpeed;
            _renderer.material.mainTextureOffset = offset;
        }

	}
}