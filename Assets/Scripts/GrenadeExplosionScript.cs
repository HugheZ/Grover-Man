using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeExplosionScript : MonoBehaviour {

    CircleCollider2D ccollider; //The collider to get the radius of the explosion
    float radius; //The radius
    float x, y; //The position in the level of the explosion
    DestructableEnviroment tileScript;

	// Use this for initialization
	void Start () {
        //Find the tilemap with the destructible tiles
        tileScript = FindObjectOfType<DestructableEnviroment>();
        //Get the radius and position of the explosion
        ccollider = GetComponent<CircleCollider2D>();
        radius = ccollider.radius;
        x = transform.position.x;
        y = transform.position.y;
        //Check if tiles are to be destroyed
        tileScript.DestroyTiles(radius, x, y);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
