using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {

    public GameObject player;   //player in the scene
    public Vector2 initialSpawnPoint;   //spawn point for first phase
    public Vector2 nextSpawnPoint;  //spawn point for other phases
    public float shootForce;    //force to launch player at other phases
    public float disableTime;   //time to remove control at other phases

	// Use this for initialization
	void Start () {
        int stage = FindObjectOfType<BossFacilitator>().stage;
        //if first stage, put player at initial spawn and activate
        if (stage == 3)
        {
            player.transform.position = initialSpawnPoint;
            player.SetActive(true);
        }
        //not at initial spawn, fling
        else
        {
            player.transform.position = nextSpawnPoint;
            StartCoroutine(Launch());
        }
	}
	
	//Disables player to allow for launching, re-enables after
    IEnumerator Launch()
    {
        //set active, disable control
        player.SetActive(true);
        player.GetComponent<PlayerController>().enabled = false;
        //launch player
        player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-.5f, .5f) * shootForce, ForceMode2D.Impulse);
        //wait for disable time
        yield return new WaitForSeconds(disableTime);
        //enable
        player.GetComponent<PlayerController>().enabled = true;
    }
}
