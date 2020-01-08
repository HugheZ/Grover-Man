using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTimer : MonoBehaviour {

    public float lifetime; //how long the laser persists
    float lifeCurrent; //current time of the laser
    public AudioSource laserFire; //fire sound of the laser

	//Reset lifeCurrent on activation
	void Awake () {
        lifeCurrent = 0;
	}

    //enables the laser fire, done through animation in order to allow arbitrary laser length
    public void StartLaserFire()
    {
        laserFire.Play();
    }
	
	// Update is called once per frame
	void Update () {
        //update time and check if time to die
        lifeCurrent += Time.deltaTime;
        if (lifeCurrent >= lifetime)
        {
            lifeCurrent = 0;
            gameObject.SetActive(false);
        }
	}
}
