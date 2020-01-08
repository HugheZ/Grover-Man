using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour {

    //Canvas to enable on startup (start game, quit, and achievements screen)
    public Canvas mainMenu;

    //Canvas to show achievements and return to main menu
    public Canvas achievementMenu;
    //achievement images, set to normal color if unlocked
    public Image image40;
    public Image imageDr;
    public Image imageSave;
    public Image imageWin;

    //achievement values
    int achievement40;
    int achievementDr;
    int achievementSave;
    int achievementWin;

    //high score text and value
    public Text txtHighScore;
    int highScore;


	// Use this for initialization
	void Start () {
        //get high score values
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        achievement40 = PlayerPrefs.GetInt("4.0", 0);
        achievementDr = PlayerPrefs.GetInt("Dr", 0);
        achievementSave = PlayerPrefs.GetInt("Save", 0);
        achievementWin = PlayerPrefs.GetInt("Win", 0);
    }
	
	//Goes to achievement menu
    public void GoToAchievements()
    {
        //set correct canvas
        mainMenu.gameObject.SetActive(false);
        achievementMenu.gameObject.SetActive(true);
        //enable achievement colors
        if(achievement40 == 1)
        {
            image40.color = Color.white;
        }
        if (achievementDr == 1)
        {
            imageDr.color = Color.white;
        }
        if (achievementSave == 1)
        {
            imageSave.color = Color.white;
        }
        if (achievementWin == 1)
        {
            imageWin.color = Color.white;
        }
        //set highs core
        txtHighScore.text = string.Format("High Score: {0}",highScore);
    }

    //Goes to main menu
    public void GoToMain()
    {
        //set correct canvas
        achievementMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    //Starts the game
    public void StartGame()
    {
        SceneManager.LoadScene("Controls");
    }

    //Quits the game
    public void Quit()
    {
        Application.Quit();
    }
}
