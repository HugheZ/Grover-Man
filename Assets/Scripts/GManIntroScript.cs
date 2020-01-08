using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManIntroScript : MonoBehaviour {

    public float runSpeed;  //run speed of dummies
    public bool readyToDie; //ready to die
    public bool player; //player bool

	// Use this for initialization
	void Start () {
        //set animation components and running speed
        GetComponent<Rigidbody2D>().velocity = new Vector2(runSpeed, 0);
        if (player)
        {
            GetComponent<Animator>().SetBool("walking", true);
            GetComponent<Animator>().SetBool("grounded", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("SeesPlayer", true);
        }
    }

    //Keeps velocity consistent
    private void Update()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(runSpeed, 0);
    }

    //If moved across the screen, kill, else do nothing
    private void OnBecameInvisible()
    {
        if (readyToDie) Destroy(gameObject);
    }

    //Once on the screen, set ready to die
    private void OnBecameVisible()
    {
        readyToDie = true;
    }
}
