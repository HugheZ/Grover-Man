using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSignal : MonoBehaviour {

    public ControlFader cf; //Used to control the light from the sky
	
    //starts a fade
	public void StartFade()
    {
        //The cutscene will engage the fading light at this point
        cf.enabled = true;
    }

    //Starts the next scene
    public void StartNextScene()
    {
        //After the player hits enter, it will load the first text
        SceneManager.LoadScene("MainToLvl1");
    }

    //Ends the final cutscene
    public void EndFinalCutscene()
    {
        //if the user has forfeited (quit), level manager will be dead
        if (LevelManager.Instance == null) Application.Quit();
        //else, reaching this point triggers the boss
        else
        {
            LevelManager.Instance.FinishedCutscene();
            SceneManager.LoadScene("BossScene");
        }
    }
}
