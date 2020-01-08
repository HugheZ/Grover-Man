using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyScript : MonoBehaviour {

    //Public variables
    public BulletScript bullet;
    public GameObject player;
    public GameObject corpse;
    public int pointValue;

    bool isLeft;
    bool isVisible = false;
    float shooterDelay;

	// Use this for initialization
	void Start () {

    }

    public void Shoot() {
        if (player != null) {
            //If the player is visible, the enemy will shoot a bullet in the x direction the player is in relation to the enemy
            if (isVisible)
            {
                if (isLeft)
                {
                    Instantiate(bullet, new Vector3((transform.position.x - 0.6f), transform.position.y, transform.position.z), Quaternion.Euler(0, 0, 180));
                }
                else
                {
                    Instantiate(bullet, new Vector3((transform.position.x + 0.6f), transform.position.y, transform.position.z), Quaternion.identity);
                }
                shooterDelay = Time.time;
            }
        }
    }

    private void Update()
    {
        if (player != null)
        {
            //The sprite will change where it is looking based on the player's x position
            if (transform.position.x <= player.transform.position.x)
            {
                isLeft = false;
                GetComponent<SpriteRenderer>().flipX = true;
            }

            if (transform.position.x >= player.transform.position.x)
            {
                isLeft = true;
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }

    private void OnBecameVisible()
    {
        //The shooter will begin his firing process when he becomes visible
        shooterDelay = Time.time - 2;
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        //The shooter will stop when he becomes invisible
        isVisible = false;
    }

    //TODO: from Zach: I noticed this code is repeated. Maybe a function call would work better

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitter = collision.gameObject;
        //If the enemy is shot, his object is deleted and he dies and a corpse is spawned
        if(collision.gameObject.tag.Equals("PlayerBullet") || collision.gameObject.tag.Equals("SniperBullet"))
        {
            float force = 10;
            if (hitter.tag == "PlayerBullet")
            {
                if (hitter.GetComponent<BulletScript>()) force = hitter.GetComponent<BulletScript>().forceApplied;
            }
            else
            {
                if (hitter.GetComponentInParent<SniperScript>()) force = hitter.GetComponentInParent<SniperScript>().forceApplied;
            }
            //spawn corpse
            GameObject corpseThrown = Instantiate(corpse, transform.position, Quaternion.identity);
            corpseThrown.GetComponent<Rigidbody2D>().AddForce((corpseThrown.transform.position - collision.gameObject.transform.position).normalized * force, ForceMode2D.Impulse);
            LevelManager.Instance.GivePoints(pointValue, false, true);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hitter = collision.gameObject;
        //Same as above, but with triggers
        if (collision.gameObject.tag.Equals("PlayerBullet") || collision.gameObject.tag.Equals("SniperBullet"))
        {
            float force = 10;
            if (hitter.tag == "PlayerBullet")
            {
                if (hitter.GetComponent<BulletScript>()) force = hitter.GetComponent<BulletScript>().forceApplied;
            }
            else
            {
                if (hitter.GetComponentInParent<SniperScript>()) force = hitter.GetComponentInParent<SniperScript>().forceApplied;
            }
            //spawn corpse
            GameObject corpseThrown = Instantiate(corpse, transform.position, Quaternion.identity);
            corpseThrown.GetComponent<Rigidbody2D>().AddForce((corpseThrown.transform.position - collision.gameObject.transform.position).normalized * force, ForceMode2D.Impulse);
            LevelManager.Instance.GivePoints(pointValue, false, true);
            Destroy(gameObject);
        }
    }
}
