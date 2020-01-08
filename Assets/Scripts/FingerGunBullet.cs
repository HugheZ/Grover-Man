using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerGunBullet : MonoBehaviour
{

    //Variable bank 
    public float speed; // bullet travel speed
    Rigidbody2D rbody;
    float startTime;    // keeps track of firing time for bullet deletion purposes
    public float lifeTime;  //duration of bullet
    public float size;
    public ParticleSystem ambientEffect;
    public ExplosionController explosion;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.velocity = transform.TransformDirection(new Vector2(speed, 0));
        startTime = Time.time;
    }

    // tracks how long the bullet has been alive and if it should be deleted
    // Update is called once per frame
    void Update()
    {
        float diff = Time.time - startTime;
        if (diff >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    //once it hits it something, it start the inevitable explosion, then deletes itself
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ambientEffect.Stop();
        Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}