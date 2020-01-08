using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableEnviroment : MonoBehaviour {

    Tilemap tilemap; //The tilemap that will be exploded

	// Use this for initialization
	void Start () {
        //Get the tilemap
        tilemap = GetComponent<Tilemap>(); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Function to check if any tiles in the explosion's radius were effected
    public void DestroyTiles(float explosionSize, float xExplosion, float yExplosion)
    {
        //Get the position vector of the explosion
        Vector3 tileImpact = Vector3.zero;

        tileImpact.x = xExplosion;
        tileImpact.y = yExplosion;
        tilemap.SetTile(tilemap.WorldToCell(tileImpact), null); //Any tiles directly on the explosion will be deleted

        //(Forgive me Dr. Valentine for I have sinned by using floats to control my loop)
        //These loops will check all positions among the radius to delete tiles
        for(float i = 0; i<=explosionSize; i += 0.1f)
        {
            for(float j=0; j<=explosionSize; j += 0.1f)
            {
                //Check up and to the right
                tileImpact.x = xExplosion + i;
                tileImpact.y = yExplosion + j;
                tilemap.SetTile(tilemap.WorldToCell(tileImpact), null);

                //Check down and to the right
                tileImpact.x = xExplosion + i;
                tileImpact.y = yExplosion - j;
                tilemap.SetTile(tilemap.WorldToCell(tileImpact), null);

                //Check up and to the left
                tileImpact.x = xExplosion - i;
                tileImpact.y = yExplosion + j;
                tilemap.SetTile(tilemap.WorldToCell(tileImpact), null);

                //Check down and to the left
                tileImpact.x = xExplosion - i;
                tileImpact.y = yExplosion - j;
                tilemap.SetTile(tilemap.WorldToCell(tileImpact), null);
            }
        }
    }
}
