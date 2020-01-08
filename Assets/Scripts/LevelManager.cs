using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    private static LevelManager _instance = null; //instance
    public Canvas UI;   //character HUD
    public Image health;    //current health indicator
    public List<Sprite> healthIcons; //health icons
    public List<Image> lifeIcons;  //life icons
    public List<Image> grenadeIcons; //Grenade Icons;
    public Text score; //current score field
    public Button restart; //restart button
    public Button end; //end button
    public GameObject eventSystem;  //event system
    public Animator sceneAnim;    //respawn animation controller
    public Image endLevelFadeOverlay;  //overlay for end level fade out
    int healthCounter;  //health counter
    int lifeCounter;    //life counter
    int hostagesSaved;  //hostages saved, used for giving health back
    int points; //point counter
    public int grenadeCounter; //grenade counter;
    public GameObject corpse; //Player's corpse

    //values for achievements
    public int maxEnemyCount;
    int enemyCount;
    public int maxHostageCount;
    int hostageCount;

    //Material check values
    public Material bricks;
    public Material dirt;
    public Material metal;
    public Material snow;
    public Material circuitry;

    //Player respawn values
    public GameObject playerPrefab; //player prefab
    Vector2 respawnPos; //position to respawn the player, NULL unless set by player death

    //Player value, set by player in level
    public PlayerController player;

    //return instance of the level manager
    public static LevelManager Instance
    {
        get
        {
            return _instance;
        }
    }

    //awake, checks for instance and destroys the new manager if multiple are inputted
    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //Start, sets core UI to don't destroy on load
    void Start()
    {
        points = 0;
        healthCounter = 0;
        lifeCounter = 3;
        if(eventSystem) DontDestroyOnLoad(eventSystem);
        if(UI) DontDestroyOnLoad(UI);
        enemyCount = 0;
        hostageCount = 0;
        grenadeCounter = 3;
    }

    //compares materials and returns a material to use for grenades
    public Material CompareGround(string name)
    {
        if (name.Contains("GCC Walls")) return bricks;
        if (name.Contains("GCC Dirt")) return dirt;
        if (name.Contains("Russian Walls")) return metal;
        if (name.Contains("Russian Snow")) return snow;
        if (name.Contains("Mecha Marx")) return circuitry;
        return null;
    }

    //resets health for next stage
    private void ResetHealth()
    {
        healthCounter = 0;
        health.sprite = healthIcons[healthCounter];
    }

    //hurt logic, decrements UI
    public void HurtPlayer()
    {
        if (healthCounter < 4)
        {
            healthCounter++;
            health.sprite = healthIcons[healthCounter];
        }
    }

    //Increments score UI
    public void GivePoints(int points, bool restock, bool isFromEnemy)
    {
        //if from enemy, increment enemy count, else increment box count
        if (isFromEnemy) enemyCount++;
        else hostageCount++;

        this.points += points;
        score.text = string.Format("Points: {0:0000}", this.points);
        if (restock) Restock();
    }

    //Restocks player health, 1 per 3 hostages saved, 1 grenade per hostage
    private void Restock()
    {
        //increment saved
        hostagesSaved++;
        //if saved 3 or more people
        if (hostagesSaved >= 3)
        {
            //reset saved count
            hostagesSaved = 0;
            //check if you can give more health, if so do so
            if (healthCounter > 0)
            {
                healthCounter--;
                health.sprite = healthIcons[healthCounter];
                //update player
                player.life++;
            }
        }
    }

    //Callable function which notifies the LevelManager that the player has reached the end of the level
    public void ReachedEndOfLevel()
    {
        sceneAnim.SetTrigger("ENDSCENE");
        Invoke("NextLevel", 2);
    }

    //Loads the next level based on this one
    private void NextLevel()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        ResetHealth();
        //if GCC, go to interscene 2
        if (activeScene.Equals(SceneManager.GetSceneByName("GroveCityCampus")))
        {
            SceneManager.LoadScene("Lvl1ToLvl2");
            FinishPlayLevel();
        }
        //else if Russia, go to end scene
        else if (activeScene.Equals(SceneManager.GetSceneByName("Russia")))
        {
            SceneManager.LoadScene("EndScene");
            FinishPlayLevel();
        }
        //else if final boss
        else if (activeScene.Equals(SceneManager.GetSceneByName("BossScene")))
        {
            //check stage, change accordingly
            int stage = FindObjectOfType<BossFacilitator>().stage;
            switch (stage)
            {
                //stage == 2, one beat, first powercore
                case 2:
                    SceneManager.LoadScene("MechaMarx1");
                    break;
                //stage == 1, two beats, second powercore
                case 1:
                    SceneManager.LoadScene("MechaMarx2");
                    break;
                //stage == 0, three beats, third powercore
                case 0:
                    SceneManager.LoadScene("MechaMarx3");
                    break;
            }
        }
        //else in the in-boss scenes, load boss scene unless boss is over
        else
        {
            //check stage, change accordingly
            int stage = FindObjectOfType<BossFacilitator>().stage;
            if (stage != 0) SceneManager.LoadScene("BossScene");
            else
            {
                FinishPlayLevel();
                SceneManager.LoadScene("WinCutscene");
            }
        }
    }

    //Player death, invokes endgame
    public void PlayerDied(Vector2 pos)
    {
        respawnPos = pos;
        transform.position = respawnPos;
        if (lifeCounter == 0) Invoke("EndGame", 2);
        else
        {
            sceneAnim.SetTrigger("RESPAWN");
            //if camera is meant to move
            if(Camera.main.GetComponent<CameraFollower>()) Invoke("MoveCamera", 1);
            Invoke("Respawn", 2);
        }
    }

    //Moves the camera, used for animation
    private void MoveCamera()
    {
        Camera.main.transform.position = new Vector3(respawnPos.x, respawnPos.y, -10);
    }

    //Handles the Grenades of the UI if a Grenade is thrown
    public void GrenadeThrown()
    {
        //A grenade icon will disappear
        grenadeCounter--;
        grenadeIcons[grenadeCounter].enabled = false;
    }

    //Handles the Grenades of the UI if a Grenade is thrown
    public void GrenadePickedUp()
    {
        if (grenadeCounter < 3)
        {
            //A grenade icon will re-appear in the UI
            grenadeIcons[grenadeCounter].enabled = true;
            grenadeCounter++;
        }
    }

    //Respawns the player, resets UI
    public void Respawn()
    {
        //lose a life
        lifeCounter--;
        //update UI
        lifeIcons[lifeCounter].enabled = false;
        ResetHealth();
        //spawn player
        GameObject playerInstance = Instantiate(playerPrefab, respawnPos, Quaternion.identity);
        Destroy(corpse);
        //link camera
        CameraFollower cf = Camera.main.GetComponent<CameraFollower>();
        if (cf != null) cf.player = playerInstance;
        //link enemies
        //TODO: very computational heavy, see if this will be needed with revamped AI behavior
        RangedEnemyScript[] rangedEnemies = FindObjectsOfType<RangedEnemyScript>();
        MeleeEnemy[] meleeEnemies = FindObjectsOfType<MeleeEnemy>();
        foreach(RangedEnemyScript re in rangedEnemies) re.player = playerInstance;
        foreach (MeleeEnemy me in meleeEnemies) me.player = playerInstance;

        //set player invincibility
        StartCoroutine(Spawn(playerInstance.GetComponent<PlayerController>()));
    }

    //Invulnerability logic for spawning
    private IEnumerator Spawn(PlayerController p)
    {
        p.invulnerable = true;
        p.GetComponent<SpriteRenderer>().color = p.invulnerableTint;
        yield return new WaitForSeconds(p.invulnerableTime);
        p.invulnerable = false;
        p.GetComponent<SpriteRenderer>().color = Color.white;
    }

    //Sets the end game UI to true, allowing you to end the game
    public void EndGame()
    {
        restart.enabled = true;
        restart.GetComponent<Image>().enabled = true;
        restart.GetComponentInChildren<Text>().enabled = true;
        end.enabled = true;
        end.GetComponent<Image>().enabled = true;
        end.GetComponentInChildren<Text>().enabled = true;
    }
	
    //On finish play level, sets hud to false
    public void FinishPlayLevel()
    {
        if(UI != null) UI.enabled = false;
    }

    //On start play level, sets hud to true
    public void FinishedCutscene()
    {
        if(UI != null) UI.enabled = true;
    }

    //On quit, destroys this object and loads to final cutscene
    public void Quit()
    {
        DestroyBossMusicIfPresent();
        SceneManager.LoadScene("FinalCutscene");
        DestroyAll();
    }

    //On restart, destroys this object and loads to first scene
    public void Restart()
    {
        DestroyBossMusicIfPresent();
        SceneManager.LoadScene("IntroScene");
        DestroyAll();
    }

    //Destroys the boss facilitator if present
    public void DestroyBossMusicIfPresent()
    {
        //get the boss facilitator, destroy if exists
        BossFacilitator bf = FindObjectOfType<BossFacilitator>();
        if (bf) Destroy(bf.gameObject);
    }

    //Sets achievement values and destroys everything
    public void WinGame()
    {
        SetAchievements();
        DestroyAll();
    }

    //Destroys all components
    private void DestroyAll()
    {
        Destroy(UI.gameObject);
        Destroy(eventSystem);
        Destroy(gameObject);
    }

    //sets achievement status
    private void SetAchievements()
    {
        //if max hostage count, set save achievement and check if also satisfy kill all enemies
        if (hostageCount >= maxHostageCount)
        {
            PlayerPrefs.SetInt("Save", 1);
            if (enemyCount >= maxEnemyCount) PlayerPrefs.SetInt("4.0",1);
        }
        //Always called at end of game, so just set win
        PlayerPrefs.SetInt("Win",1);

        //check new high score
        int previousHigh = PlayerPrefs.GetInt("HighScore", 0);
        if (points > previousHigh) PlayerPrefs.SetInt("HighScore", points);
    }
}
