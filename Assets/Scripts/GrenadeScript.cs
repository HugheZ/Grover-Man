using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour {

    public float detonateTimer; //Time grenade will have till it explodes
    public GameObject explosion; //The explosion
    float pinPulled; //How long between the grenade thrown and the explosion
    public AudioClip ticTicBang;    //audiofile for ticking grenade
    public AudioSource expSound;

	// Use this for initialization
	void Start () {
        pinPulled = Time.time; //Set the pin to be pulled for the explosion
        expSound.PlayOneShot(ticTicBang);   //unless im mistaken, grenades should tick for a bit, then explode
	}
	
	// Update is called once per frame
	void Update () {
        //If enough time has passed, blow the grenade up
		if(Time.time - pinPulled >= detonateTimer)
        {
            blowUp();
        }
	}

    private void blowUp()
    {
        //Instantiate an explosion
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
