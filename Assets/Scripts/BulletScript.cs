using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;
    Rigidbody2D rbody;
    float startTime;
    public float forceApplied;

    // Use this for initialization
    void Start()
    {
        //Initialize the rigid body and the time till destruction
        rbody = GetComponent<Rigidbody2D>();
        rbody.velocity = transform.TransformDirection(new Vector2(speed, 0));
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //After a certain period of time, destroy the bullet
        float diff = Time.time - startTime;
        if (diff >= 4)
        {
            Destroy(gameObject);
        }
    }

    //Destroy the bullet on collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    //Destroy the bullet if it goes off camera
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
