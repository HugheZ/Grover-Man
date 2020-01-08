using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *This code controls how long the sniper bullet is visible / sticks around in world 
 */
public class SniperScript : MonoBehaviour {

    float startTime;
    public float lifeSpan;
    public float colliderSpan;
    public float forceApplied;

    // Use this for initialization
    void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float currentTime = Time.time - startTime;
        SpriteRenderer rend = GetComponentInChildren<SpriteRenderer>();
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 1 - currentTime / lifeSpan);   //makes the "bullet" fade out
        if (currentTime >= lifeSpan) Destroy(gameObject);
        if (currentTime >= colliderSpan) GetComponentInChildren<BoxCollider2D>().enabled = false;
	}
}
