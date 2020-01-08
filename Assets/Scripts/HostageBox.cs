using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageBox : MonoBehaviour {

    //Public variables
    public AudioClip helpF;
    public AudioClip helpM;

    //Determine what kind of hostage it is
    public int hostagePoints;
    public GameObject hostage1;
    public GameObject hostage2;
    public GameObject theDoctor;
    LevelManager lm;
    public int choice;
    bool notPlayed;
    float lastPlayed;
    private void Start()
    {
        lm = LevelManager.Instance;
        notPlayed = true;
        lastPlayed = Time.time;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the player collides with the box, delete the box and instantiate the hostage prefab
        if(collision.gameObject.tag == "Player")
        {
            if (choice == 0) Instantiate(hostage1, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
            else if (choice == 1) Instantiate(hostage2, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
            else
            {
                Instantiate(theDoctor, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
                //the doctor, unlock achievement
                PlayerPrefs.SetInt("Dr", 1);
            }
            lm.GrenadePickedUp();
            lm.GivePoints(hostagePoints, true, false);
            Destroy(gameObject);
        }
    }

    //Play a help sound when the player sees the box, and limits voiceline spam
    private void OnBecameVisible()
    {
        print (Time.time - lastPlayed);
        if (notPlayed)
        {
            if (choice == 0)
            {
                GetComponent<AudioSource>().PlayOneShot(helpM);
            }
            else if (choice == 1)
            {
                GetComponent<AudioSource>().PlayOneShot(helpF);
            }
            lastPlayed = Time.time;
            notPlayed = false;
        }
        //lets audio be played again after enough time away from box
        else if (Time.time-lastPlayed > 2.0f)
        {
            notPlayed = true;
        }
    }
}
