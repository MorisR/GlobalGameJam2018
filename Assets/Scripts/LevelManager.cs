using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

   // public List<Wave> waves;  

    public void LoadMainScene()
    {
        SceneManager.LoadScene("main2");
    }

    public void LoadCredScene()
    {
        SceneManager.LoadScene("Credits");
    }

    // Use this for initialization
    void Start () {
        GameObject.DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	}







}
