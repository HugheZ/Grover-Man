using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody2D rb; //player rigidbody
    //Bounds size;    //bounds of the player collider
    Animator anim;  //player animator
    Gun gun;        //player gun script
    ParticleSystem blood;   //blood system
    LevelManager gm;        //level manager
    float gunScale;         //scale of the gun, used for animation
    float gunPos;           //gun postion, used for animation
    float lastGroundTime;   //last grounded time
    Vector2 lastGroundPos;  //sets grounded position for respawning
    public float maxSpeed;  //maximum speed
    public float groundGraceTime;   //grace time for rejumping
    public int life;    //health
    public bool left;   //facing left
    public float stunTime;  //time for stun
    public float invulnerableTime;  //time for invulnerability
    public bool invulnerable;   //is invulnerable
    public float knockback; //knockback force
    public float speed; //player speed
    public float jumpForce; //player jump force
    public float jumpGravFactor;    //player gravity factor
    public bool grounded;   //is grounded
    public bool startedJump;    //jump started
    public bool jump;   //currently jumping
    public bool falling;    //currently falling
    public bool crouch;     //currently crouched
    public float jumpForceApplied;  //actually applied jump force
    public Color stunTint;  //tint of stunned
    public Color invulnerableTint;  //tint of invulnerability
    public AudioClip oof;   //hurt clip
    public AudioSource oofSource;   //hurt clip source
    public GameObject cape; //cape
    public GameObject corpse;   //corpse to spawn on death
    public LayerMask groundLayer;   //layer to collide when jumping
    public LayerMask platformLayer; //layer to collide when jumping but not checking above
    public LayerMask enemyLayer;    //layer to allow jumping on enemies
    LayerMask jumpLayer; //mask to use for jumping
    LayerMask aboveLayer; //mask to use for above hits

    //vectors for crouching collider change
    public Vector2 offsetCrouch; //offset to drop cape and gun by
    public Vector2 sizeCrouch; //size after crouching
    Vector2 offsetRegular; //regular offset
    Vector2 sizeRegular; //regular size

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        //size = GetComponent<BoxCollider2D>().bounds;
        anim = GetComponent<Animator>();
        blood = GetComponent<ParticleSystem>();
        jumpForceApplied = 0;
        gun = GetComponent<Gun>();
        gunPos = gun.gunLoc.transform.localPosition.x;
        gunScale = gun.gunLoc.transform.localScale.x;
        offsetRegular = GetComponent<BoxCollider2D>().offset;
        sizeRegular = GetComponent<BoxCollider2D>().size;

        //set layermasks
        jumpLayer = groundLayer | enemyLayer | platformLayer;
        aboveLayer = groundLayer | enemyLayer;

        //set level manager last to avoid bricking on testing levels
        gm = LevelManager.Instance;
        gm.player = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if you press escape, kill
        if (Input.GetButtonDown("Extract")) Die();

        //check for grounded
        grounded = IsGrounded() && !jump;
        //reset bools and animation clips if grounded
        if (grounded)
        {
            jump = false;
            falling = false;
            anim.SetBool("grounded", true);
            lastGroundTime = Time.time;
            lastGroundPos = transform.position;
        }//else set animation bool to not grounded
        else anim.SetBool("grounded", false);

        //if walked off a cliff, set flags and vertical force
        if (!grounded && !jump && !falling)
        {
            jumpForceApplied = 0;
            falling = true;
        }

        //if grounded and jump button is hit while not already jumping, jump
        if (Input.GetButtonDown("Jump") && WasGrounded() && !jump && !crouch)
        {
            startedJump = true;
            anim.SetBool("grounded", grounded);
        }
        //if jump button was released while going up, half the jump force
        else if (Input.GetButtonUp("Jump") && jump && rb.velocity.y > 0)
        {
            jumpForceApplied /= 1.5f;
            jump = false;
            falling = true;
        }
        //set flags and bools for moving right while previously going left
        if (left && Input.GetAxis("Horizontal") > 0)
        {
            left = false;
            GetComponent<SpriteRenderer>().flipX = false;
            gun.gunLoc.transform.localScale = new Vector2(gunScale, gun.gunLoc.transform.localScale.y);
            gun.gunLoc.transform.localPosition = new Vector2(gunPos, gun.gunLoc.transform.localPosition.y);
        }
        //set flags and bools for moving left while previously going right
        else if (!left && Input.GetAxis("Horizontal") < 0)
        {
            left = true;
            GetComponent<SpriteRenderer>().flipX = true;
            //gun.gunLoc.flipX = true;
            gun.gunLoc.transform.localScale = new Vector2(-gunScale, gun.gunLoc.transform.localScale.y);
            gun.gunLoc.transform.localPosition = new Vector2(-gunPos, gun.gunLoc.transform.localPosition.y);
        }

        //if crouching, set crouch values
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
            anim.SetBool("crouch", crouch);
            //change cape height
            cape.transform.localPosition = new Vector2(0,.15f);
            //change gun height
            float x;
            if (left) x = -.14f;
            else x = .14f;
            gun.gunLoc.transform.localPosition = new Vector2(x, -.04f);
            //change collider size
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.offset = offsetCrouch;
            collider.size = sizeCrouch;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
            anim.SetBool("crouch", crouch);
            //change cape height
            cape.transform.localPosition = new Vector2(0, .33f);
            //change gun height, different if right or left
            float x;
            if (left) x = -.05f;
            else x = .05f;
            gun.gunLoc.transform.localPosition = new Vector2(x, .12f);
            //change collider size
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.offset = offsetRegular;
            collider.size = sizeRegular;
        }


        //animation control
        if (Input.GetAxis("Horizontal") != 0 && grounded && !crouch) anim.SetBool("walking", true);
        else anim.SetBool("walking", false);
        
	}

    private void FixedUpdate()
    {
        //apply vertical motion
        Vector2 direc = new Vector2(0, jumpForceApplied);

        //if jump started, apply force and set flags
        if (startedJump)
        {
            startedJump = false;
            if (!PlatformAbove())
            {
                jumpForceApplied = jumpForce;
                grounded = false;
                falling = false;
                jump = true;
            }
        }
        //if falling or jumping, degrade jumping speed
        if (jump)
        {
            jumpForceApplied -= Time.deltaTime * .8f * jumpGravFactor;
        }
        else if(falling)
        {
            jumpForceApplied -= Time.deltaTime * jumpGravFactor;
        }
        //if velocity is less than zero, set falling state
        if (rb.velocity.y < 0)
        {
            jump = false;
            falling = true;
        }
        
        //if going up and hit a platform above
        if (direc.y > 0)
        {
            if (PlatformAbove())
            {
                jump = false;
                falling = true;
                direc.y = 0;
                jumpForceApplied = 0;
            }
        }

        //apply horizontal motion if not crouched
        if (!crouch)
        {
            //reduce vector to only allow a max speed of given speed
            direc = new Vector2(Input.GetAxis("Horizontal") * Time.deltaTime * speed, jumpForceApplied);
            if (direc.y > maxSpeed) direc.y = maxSpeed;
        }
        //set speed
        rb.velocity = direc;
    }

    //checks if hit from above
    private bool PlatformAbove()
    {
        Bounds size = GetComponent<BoxCollider2D>().bounds;
        Vector3 positionl = new Vector2(size.min.x, size.max.y);
        Vector3 positionr = size.max;
        RaycastHit2D hitl = Physics2D.Raycast(positionl, Vector2.up, .1f, aboveLayer);
        RaycastHit2D hitr = Physics2D.Raycast(positionr, Vector2.up, .1f, aboveLayer);
        return (hitl.collider != null || hitr.collider != null);

    }

    //checks if grounded by boxcast to feet
    private bool IsGrounded()
    {
        Bounds size = GetComponent<BoxCollider2D>().bounds;
        Vector3 positionl = size.min;
        Vector3 positionr = new Vector2(size.max.x, size.min.y);
        RaycastHit2D hitl = Physics2D.Raycast(positionl, Vector2.down, .1f, jumpLayer);
        RaycastHit2D hitr = Physics2D.Raycast(positionr, Vector2.down, .1f, jumpLayer);
        return (hitl.collider != null || hitr.collider != null);

    }

    //check if grounded by time last grounded
    private bool WasGrounded()
    {
        return (Time.time - lastGroundTime) < groundGraceTime;
    }

    //logic for getting hit
    private void Hit(GameObject hitter)
    {
        //play blood effect, drop the gun, and decrease life
        blood.Play(false);
        gun.HurtGunDrop();
        life--;
        //apply stun force
        rb.velocity = Vector2.zero;
        rb.AddForce((transform.position - hitter.transform.position).normalized * knockback, ForceMode2D.Impulse);
        gm.HurtPlayer();
        //if dead, kill, else stun
        if (life == 0) Die();
        else StartCoroutine("Stun");
    }

    //kills the player and informs the game manager
    private void Die()
    {
        GameObject body = Instantiate(corpse, transform.position, Quaternion.identity);
        gm.corpse = body;
        gm.PlayerDied(lastGroundPos);
        Destroy(gameObject);
    }

    //collision logic
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collision with enemy, hurt player, else if with weapon pickup, give new weapon
        if (collision.gameObject.tag == "Enemy" && !invulnerable) Hit(collision.gameObject);
        else if (collision.gameObject.tag == "WeaponPickup")
        {
            gun.ChangeGun(collision.gameObject.GetComponent<GunGiver>().gunIndex);
            Destroy(collision.gameObject);
        }
    }

    //Stun logic
    private IEnumerator Stun()
    {
        //reset player control and set animation values and color values
        oofSource.PlayOneShot(oof);
        invulnerable = true;
        this.enabled = false;
        anim.SetBool("hurt", true);
        GetComponent<SpriteRenderer>().color = stunTint;
        //not stunned, give invulnerability and set color
        yield return new WaitForSeconds(stunTime);
        this.enabled = true;
        anim.SetBool("hurt", false);
        GetComponent<SpriteRenderer>().color = invulnerableTint;
        yield return new WaitForSeconds(invulnerableTime);
        //set gameplay to regular status after Iframes
        invulnerable = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    //trigger enter logic
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if its with the end level trigger, next level, disable input and hurt, play animation loop
        if (collision.gameObject.tag.Equals("EndLevelTrigger"))
        {
            gm.ReachedEndOfLevel();
            anim.SetBool("endscene", true);
            invulnerable = true;
            gun.gunLoc.enabled = false;
            gun.enabled = false;
            enabled = false;
        }
        //if it's an enemy, hurt
        else if (collision.gameObject.tag.Equals("Enemy") && !invulnerable) Hit(collision.gameObject);
    }
}
