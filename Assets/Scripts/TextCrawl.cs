using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCrawl : MonoBehaviour {

    public float crawlSpeed;    //crawl speed of the text

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, crawlSpeed);
	}
}
