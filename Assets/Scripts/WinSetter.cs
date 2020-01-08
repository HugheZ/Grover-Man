using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinSetter : MonoBehaviour {

    public GameObject UI; //UI to enable
    public Text achievementText; //achievement box in UI to enable

    int achievement40;  //4.0 achievement value holder
    int achievementDr; //Dr achievement value holder
    int achievementSave; //Savrior achievement value holder
    int achievementWin; //Win achievement value holder
    int highScore; //high score achievement value holder

	// Use this for initialization
	void Start () {
        //destroy the boss controller to stop music
        Destroy(FindObjectOfType<BossFacilitator>().gameObject);
        achievement40 = PlayerPrefs.GetInt("4.0",0);
        achievementDr = PlayerPrefs.GetInt("Dr", 0);
        achievementSave = PlayerPrefs.GetInt("Save", 0);
        achievementWin = PlayerPrefs.GetInt("Win", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        //first find game manager and tell it to record achievements
        LevelManager.Instance.WinGame();
	}
	
	//Enables win cutscene UI
    public void EnableUI()
    {
        //activate UI
        UI.SetActive(true);
        //if any achievement value is different now, it must be
        //  because an achievement was unlocked this game, display message
        if(PlayerPrefs.GetInt("4.0",0) != achievement40 || PlayerPrefs.GetInt("Dr",0) != achievementDr ||
            PlayerPrefs.GetInt("Save",0) != achievementSave || PlayerPrefs.GetInt("Win",0) != achievementWin)
        {
            achievementText.enabled = true;
        }
    }

    //Restarts
    public void RestartGame()
    {
        SceneManager.LoadScene("IntroScene");
    }

    //Quits game
    public void QuitGame()
    {
        Application.Quit();
    }
}
