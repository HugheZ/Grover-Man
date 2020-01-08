using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCoreScript : MonoBehaviour {

    public int life; //How much life the core will have
    public Color[] lifeColor; //The tint that the core will get the more it is hit

    //The explosions it will generate when it dies
    public GameObject explosion; 
    public GameObject bigBoom;
    public LevelManager lm;

    //The end level trigger once its gone
    public GameObject endLevelTrigger;

    public GameObject player;

    //When the coroutine becomes active, set this to true so the explosions dont keep going
    bool coroutineActive = false;

	// Use this for initialization
	void Start () {
        lm = LevelManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        //Blow the thing up if its life is zero and its not blowing up currently
		if(life == 0 && coroutineActive == false)
        {
            StartCoroutine("destroyCore");
            coroutineActive = true;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If a bullet hits it, take a life off and add a redder tint
        if ((collision.gameObject.tag.Equals("PlayerBullet") || collision.gameObject.tag.Equals("SniperBullet")) && life!=0)
        {
            life--;
            GetComponent<SpriteRenderer>().color = lifeColor[life];
        }
    }

    //Blow the core up
    private IEnumerator destroyCore()
    {
        //Make a small explosion...
        Instantiate(explosion, transform.position, transform.rotation);
        yield return new WaitForSeconds(1);

        //...and another...
        Instantiate(explosion, new Vector3(transform.position.x, transform.position.y+1, 0), transform.rotation);
        yield return new WaitForSeconds(1);

        //...and another
        Instantiate(explosion, new Vector3(transform.position.x, transform.position.y-1, 0), transform.rotation);
        yield return new WaitForSeconds(3);

        //Make a big explosion to destroy it
        Instantiate(bigBoom, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);

        //Set a large endLevelTrigger to get GroverMan back to the boss fight
        GameObject endLevel = Instantiate(endLevelTrigger, new Vector3(lm.player.GetComponent<Rigidbody2D>().position.x, lm.player.GetComponent<Rigidbody2D>().position.y, 0), lm.player.GetComponent<Rigidbody2D>().transform.rotation);
        endLevel.transform.localScale = new Vector3(100, 100, 0);

        //Destroy the core in the blinding light
        Destroy(gameObject);
    }
}
