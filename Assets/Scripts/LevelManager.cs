using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    static LevelManager _instance;

    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new LevelManager();
            return _instance;
        }
    }

    // public List<Wave> waves;  

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

  
    // Use this for initialization
    void Start () {
	}







}
