using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

    public float explosionTime; //time for explosion to last

    // Use this for initialization
    //Kill after explosion time
    void Start () {
        Destroy(gameObject, explosionTime);
	}
}
