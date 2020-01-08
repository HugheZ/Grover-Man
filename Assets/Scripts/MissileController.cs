using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour {

    public GameObject explosion; //explosion to instantiate
    public float speed; //speed of the missile

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector2.down * speed);
	}

    //Handles collision and explosion
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
