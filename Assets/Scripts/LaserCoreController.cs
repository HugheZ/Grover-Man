using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCoreController : MonoBehaviour {

    public Transform player; //player to follow
    public float lifetime; //how long the core persists
    float lifeCurrent; //current time of the core

    public float speed; //how fast the core moves
    public float turnSpeed; //how fast the core turns

	// Use this for initialization
	void Start () {
        lifeCurrent = 0;
        player = LevelManager.Instance.player.transform;
    }

    //Resets the player on set active
    private void OnEnable()
    {
        lifeCurrent = 0;
        player = LevelManager.Instance.player.transform;
    }

    // Update is called once per frame
    void Update () {
        //update lifetime
        lifeCurrent += Time.deltaTime;
        if (lifeCurrent >= lifetime)
        {
            lifeCurrent = 0;
            gameObject.SetActive(false);
        }
        //if it has a player reference, follow it
        if (player)
        {
            //transform position by speed
            transform.position = Vector3.Lerp(transform.position, transform.TransformPoint(Vector3.right), speed * Time.deltaTime);
            //get angle to rotate
            Vector2 dist = player.position - transform.position;
            float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);
        }
	}

    //Handles collision, deactivate if collided with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            lifeCurrent = 0;
            gameObject.SetActive(false);
        }
    }
}
