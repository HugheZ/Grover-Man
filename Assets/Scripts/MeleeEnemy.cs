using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls behavior and animation of the melee enemies
public class MeleeEnemy : MonoBehaviour {

    //should be set to the current instance of Grover Man
    public GameObject player;
    public GameObject corpse;
    public LayerMask groundLayer;
    public int pointValue;  //how many points the enemy is worth for killing

    public float x1, x2;
    public float y1, y2;
    bool activated = false;

    //enemy elements for quick use later on
    Rigidbody2D rbody;
    public Animator anim;

    bool hit;   //hitting player
    public float speed; //movement speed for enemy chasing player
    public float patrolSpeed;   //walk speed during idle patrolling
    float pauseStart;
    bool isVis; //keeps track of if it can "be seen" so it know when to start trying to get the player
    bool facingLeft;    //am I facing left, or right
    float segTime; //controls
    float lastChange;   //last time the patrol state was affected
    Vector2 idleV;  //velocity for this patrol state
    bool theEdge;   //am I over an edge

    //keeps tracks of when the enemy entered stun, and how long it will remain in stun
    float stunDur;
    float stunStart;
    
    //help control animation
    public float spacing;
    Vector2 playerLocation;

    //Stun effects
    public float knockback; //knockback force
    public int life;    //health
    public Color hitTint;  //tint of stunned

    // Use this for initialization
    void Start () {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        pauseStart = 0;
        isVis = false;
        hit = false;
        facingLeft = true;
        segTime = 2.0f;
        idleV = new Vector2(0, 0);
        theEdge = false;
	}

    // Update is called once per frame
    //deals with movement and animation of the melee enemy
    void Update()
    {
        if (player != null)
        {
            //checks for a lock on the player after initial line of sight is established
            if (isVis)
            {
                if (!haveLock())
                {
                    isVis = false;
                }
            }
            //initial line of sight established
            else if (canSeePlayer())
            {
                isVis = true;
            }
            //updates the player position
            playerLocation = transform.InverseTransformPoint(player.transform.position);
            //controls animation of melee enemies
            if (!isVis && (Time.time - lastChange) > segTime)
            {
                //if the enemy is moving, make it stop until the end of the next time segment
                //STATE: Pause and "look around" for a bit
                if (rbody.velocity.x != 0)
                {
                    idleV = new Vector2(0, rbody.velocity.y);
                    anim.SetBool("SeesPlayer", false);
                }
                //if the enemy is stopped, turn him around and make him move that direction until end of next time segment
                else if (rbody.velocity.x == 0)
                {
                    facingLeft = !facingLeft;
                    //STATE: Patrol Left
                    if (facingLeft)
                    {
                        idleV = new Vector2(-patrolSpeed, rbody.velocity.y);
                        GetComponent<SpriteRenderer>().flipX = false;
                        //this lets the enemy "walk", it can't actually see the player
                        anim.SetBool("SeesPlayer", true);
                    }
                    //STATE: Patrol Right
                    else if (!facingLeft)
                    {
                        idleV = new Vector2(patrolSpeed , rbody.velocity.y);
                        GetComponent<SpriteRenderer>().flipX = true;
                        anim.SetBool("SeesPlayer", true);
                    }
                }
                //set to track timings
                lastChange = Time.time;
            }
            //STATE: Chasing Player
            else if (isVis && activated && player.GetComponent<PlayerController>().enabled)
            {
                anim.SetBool("SeesPlayer", true);

                float direction = gameObject.GetComponent<Transform>().position.x - player.gameObject.GetComponent<Transform>().position.x;
                //determines which way the enemy should be facing
                if (direction > -spacing)
                {
                    rbody.velocity = new Vector2(-speed, rbody.velocity.y);
                    GetComponent<SpriteRenderer>().flipX = false;
                    facingLeft = true;
                }
                if (direction < spacing)
                {
                    rbody.velocity = new Vector2(speed, rbody.velocity.y);
                    GetComponent<SpriteRenderer>().flipX = true;
                    facingLeft = false;
                }
                if (direction < spacing && direction > -spacing)
                {
                    rbody.velocity = new Vector2(0, rbody.velocity.y);
                    anim.SetBool("SeesPlayer", false);
                }


            }
            //if not chasing the player, re-ups velocity after checking to make sure its not running off a platform
            if (!isVis)
            {
                Vector2 down = new Vector2(transform.position.x, transform.position.y);
                RaycastHit2D dhit = Physics2D.Raycast(down, Vector2.down, 1.1f, groundLayer);
                //whoa there, that be a cliff you running towards. how about you just stop this part of your patrol and take a nice break
                if (!theEdge && dhit.collider == null)
                {
                    idleV = new Vector2(0, rbody.velocity.y);
                    lastChange = Time.time;
                    anim.SetBool("SeesPlayer", false);
                    theEdge = true;
                }
                //lets the melee enemy move off the edge before starting to seriously check for the edge again
                else if (theEdge && dhit.collider != null)
                {
                    theEdge = false;
                }
                //allows enemy that lost sight while falling (somehow) to just fall to the ground
                if (!(rbody.velocity.x == 0 && idleV.x == 0))
                {
                    rbody.velocity = idleV;
                }
                else
                {
                    anim.SetBool("SeesPlayer", false);
                }
            }
            //else if (!hit) anim.SetBool("SeesPlayer", false);

            //determines if the enemy should try to move towards groverman
            float pX = playerLocation.x;
            float pY = playerLocation.y;
            if ((pX >= x1 && pX <= x2) && (pY >= y1 && pY <= y2))
            {
                activated = true;
            }
        }
        else
        {
            anim.SetBool("SeesPlayer", false);
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }
    }

    //deals with collisions for the player and the bullets
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitter = collision.gameObject;
        //hits player, and is then stunned so player can be chain hit to death
        if (hitter.tag == "Player")
        {
            rbody.velocity = new Vector2(0, rbody.velocity.y);
            stunStart = Time.time;
            hit = true;
            anim.SetBool("Hit", hit);
        }
        //deals with killing the melee enemy
        else if(hitter.tag == "PlayerBullet" || hitter.tag == "SniperBullet")
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
            GameObject corpseThrown = Instantiate(corpse, rbody.position, Quaternion.identity);
            corpseThrown.GetComponent<Rigidbody2D>().AddForce((corpseThrown.transform.position - collision.gameObject.transform.position).normalized * force, ForceMode2D.Impulse);
            LevelManager.Instance.GivePoints(pointValue, false, true);
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        hit = false;
        anim.SetBool("Hit", hit);
    }


    //when you spawn, call this and send it the instance of GroverMan
    public void SetPlayer(GameObject groverMan)
    {
        player = groverMan;
    }

    //TODO: from Zach: I noticed this code is repeated. Maybe a function call would work better
    //kills enemy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hitter = collision.gameObject;
        //deals with killing the melee enemy
        if (hitter.tag == "PlayerBullet" || hitter.tag == "SniperBullet")
        {
            float force = 10;
            if (hitter.tag == "PlayerBullet")
            {
                if(hitter.GetComponent<BulletScript>()) force = hitter.GetComponent<BulletScript>().forceApplied;
            }
            else
            {
                if(hitter.GetComponentInParent<SniperScript>()) force = hitter.GetComponentInParent<SniperScript>().forceApplied;
            }
            GameObject corpseThrown = Instantiate(corpse, rbody.position, Quaternion.identity);
            corpseThrown.GetComponent<Rigidbody2D>().AddForce((corpseThrown.transform.position - collision.gameObject.transform.position).normalized * force, ForceMode2D.Impulse);
            LevelManager.Instance.GivePoints(pointValue, false, true);
            Destroy(gameObject);
        }
    }

    private void Hit(GameObject hitter)
    {
        life--;
        //apply stun force
        rbody.velocity = Vector2.zero;
        rbody.AddForce((transform.position - hitter.transform.position).normalized * knockback, ForceMode2D.Impulse);
        //if dead, kill, else stun
        if (life == 0) Die();
        else StartCoroutine("Stun");
    }

    private void Die()
    {
        Instantiate(corpse, rbody.position, Quaternion.identity);
        LevelManager.Instance.GivePoints(pointValue, false, true);
        Destroy(gameObject);
    }
    private IEnumerator Stun()
    {
        //reset player control and set animation values and color values
        this.enabled = false;
        GetComponent<SpriteRenderer>().color = hitTint;
        yield return new WaitForSeconds(5);
        this.enabled = true;
        //set gameplay to regular status after Iframes
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    //checks directly "infront of" enemy to see if they can see the player
    private bool canSeePlayer()
    {
        Vector2 bottom = new Vector2(transform.position.x, transform.position.y - 0.5f);
        Vector2 top = new Vector2(transform.position.x, transform.position.y + 0.5f);
        RaycastHit2D bhit = Physics2D.Raycast(bottom, Vector2.right, 10.0f, groundLayer);
        RaycastHit2D thit = Physics2D.Raycast(top, Vector2.right, 10.0f, groundLayer);
        if (facingLeft)
        {
            bhit = Physics2D.Raycast(bottom, Vector2.left, 10.0f, groundLayer);
            thit = Physics2D.Raycast(top, Vector2.left, 10.0f, groundLayer);
        }
        return ((bhit && bhit.collider.gameObject.GetComponent<PlayerController>() )
            || (thit && thit.collider.gameObject.GetComponent<PlayerController>()));
    }

    //after it has "seen" the player directly infront of it, it will try to look directly at the player so that the player
    //can't really just break line of sight by jumping, or ducking
    private bool haveLock()
    {
        Vector2 Lock = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D locked = Physics2D.Raycast(Lock, new Vector2(player.transform.position.x - transform.position.x,
                player.transform.position.y - transform.position.y), 5f, groundLayer);
        if (locked && locked.collider.gameObject.GetComponent<PlayerController>())
        {
            return true;
        }
        return false;
    }
}
