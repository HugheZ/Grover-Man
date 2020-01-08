using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour {

    public string nextScene;    //next scene to load
    public Text pressStartText; //press start text
    public float flashDuration; //flash duration of 
    float currentTime;  //current time
    float startTime;    //start time

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        //set current time
        currentTime = Time.time - startTime;
        //flash 'press start'  on duration
        if (currentTime >= flashDuration)
        {
            pressStartText.enabled = !pressStartText.enabled;
            startTime = Time.time;
        }

        //if input is hit, call finish cutscene and load next scene
        if (Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene(nextScene);
            if (LevelManager.Instance != null) LevelManager.Instance.FinishedCutscene();
        }
	}
}
