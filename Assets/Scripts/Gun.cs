using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public LevelManager manager;

    public AudioClip bang; //fingergun sound
    public AudioClip sniper; //sniper sound
    public AudioClip shotgun; //shotgun sound
    public AudioClip pistol; //pistol sound
    public AudioSource gunSource; //gun sound

    public ParticleSystem gunExplosion; //Particles for the smoke from the gun
    public BulletScript baseBullet; //Base bullets
    public BulletScript shotgunBullet; //shotgun bullet
    public GameObject sniperBullet; //sniper bullet
    public FingerGunBullet fgb; //finger gun bullet
    public int gunIndex; //To keep track of what gun is being used
    public float baseDistance; //Distance to spawn from player
    public float bulletHeight; //How high above the ground it will be
    public float vOffset; //Vertical offset to gunLoc vertical location
    public float cooldown; //How long before the bullet can be shot again

    public GameObject grenade; //Grenade object
    public float grenadeCooldown; //How long before another grenade can be thrown
    public float grenadeDistance; //Distance to spawn grenade from player
    public float cooldown2;

    public List<Sprite> guns; //Sprites for each gun

    public SpriteRenderer gunLoc; //Gun location on the player

    //gun based cooldown sets
    public float gun0Cooldown;
    public float gun1Cooldown;
    public float gun2Cooldown;
    public float gun3Cooldown;

	// Use this for initialization
	void Start () {
        //Set the sprite of the gun to whatever the gun is
        gunLoc.sprite = guns[gunIndex];
        manager = FindObjectOfType<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //Determine when the user can shoot the gun
        if (cooldown > 0) cooldown -= Time.deltaTime;
        else if (cooldown < 0) cooldown = 0;

        if (cooldown2 > 0) cooldown2 -= Time.deltaTime;
        else if (cooldown2 < 0) cooldown2 = 0;
        //Key input for firing the gun
        if (Input.GetButtonDown("Fire"))
        {
            bulletHeight = gunLoc.transform.localPosition.y + vOffset;
            Shoot(GetComponent<PlayerController>().left);
        }

        //Grenade Control
        if (Input.GetButtonDown("Grenade") && manager.grenadeCounter > 0)
        {
            bulletHeight = gunLoc.transform.localPosition.y + vOffset;
            ThrowGrenade(GetComponent<PlayerController>().left);
        }
    }

    //Drop the gun if the user is hurt
    public void HurtGunDrop()
    {
        gunIndex = 0;
        ChangeGun(0);
    }

    //Change the gun if the user picks one up
    public void ChangeGun(int index)
    {
        if (index >= 0 && index <= guns.Count)
        {
            gunIndex = index;
            gunLoc.sprite = guns[index];
            cooldown = 0;
        }
    }

    //method to shoot specific guns
    private void Shoot(bool left)
    {
        if(gunIndex == 0)
        {
            ShootGun0(left);
        }
        //pistol
        else if(gunIndex == 1)
        {
            ShootGun1(left);
        }
        //sniper
        else if(gunIndex == 2)
        {
            ShootGun2(left);
        }
        //shotgun
        else if(gunIndex == 3)
        {
            ShootGun3(left);
        }
    }

    //Code to instantiate the bullets for the pistol
    private void ShootGun0(bool left)
    {
        if (cooldown <= 0)
        {
            gunSource.PlayOneShot(pistol);
            if (!left)
            {
                Instantiate(baseBullet, transform.TransformPoint(new Vector3(baseDistance, bulletHeight, 0)), Quaternion.identity);
            }
            else Instantiate(baseBullet, transform.TransformPoint(new Vector3(-baseDistance, bulletHeight, 0)), Quaternion.Euler(0, 0, 180));
            cooldown = gun0Cooldown;
            //Play the smoke effect
            gunExplosion.Play();
        }
    }

    //Code for shooting the sniper rifle and generating the sniper bullet
    private void ShootGun1(bool left)
    {
        if (cooldown <= 0)
        {
            gunSource.PlayOneShot(sniper);
            if (!left) Instantiate(sniperBullet, transform.TransformPoint(new Vector3(baseDistance, bulletHeight, 0)), Quaternion.identity);
            else Instantiate(sniperBullet, transform.TransformPoint(new Vector3(-baseDistance, bulletHeight, 0)), Quaternion.Euler(0, 0, 180));
            cooldown = gun1Cooldown;
            //Play the smoke effect
            gunExplosion.Play();
        }
    }

    //Code for shooting the shotgun and generating the three shotgun bullets
    private void ShootGun2(bool left)
    {
        if(cooldown <= 0)
        {
            //The shotgun will instantiate three bullets in an arc
            gunSource.PlayOneShot(shotgun);
            if (!left)
            {
                Instantiate(shotgunBullet, transform.TransformPoint(new Vector3(baseDistance, bulletHeight + .2f, 0)), Quaternion.Euler(0,0,10));
                Instantiate(shotgunBullet, transform.TransformPoint(new Vector3(baseDistance, bulletHeight, 0)), Quaternion.identity);
                Instantiate(shotgunBullet, transform.TransformPoint(new Vector3(baseDistance, bulletHeight - .2f, 0)), Quaternion.Euler(0,0,-10));
            }
            else
            {
                Instantiate(shotgunBullet, transform.TransformPoint(new Vector3(-baseDistance, bulletHeight - .2f, 0)), Quaternion.Euler(0, 0, 190));
                Instantiate(shotgunBullet, transform.TransformPoint(new Vector3(-baseDistance, bulletHeight, 0)), Quaternion.Euler(0, 0, 180));
                Instantiate(shotgunBullet, transform.TransformPoint(new Vector3(-baseDistance, bulletHeight + .2f, 0)), Quaternion.Euler(0, 0, 170));
            }
            cooldown = gun2Cooldown;
            //Play the smoke effect
            gunExplosion.Play();
        }
    }

    //Code for shooting the finger gun
    private void ShootGun3(bool left)
    {
        if (cooldown <= 0)
        {
            gunSource.PlayOneShot(bang);
            if (!left)
            {
                Instantiate(fgb, transform.TransformPoint(new Vector3(baseDistance, bulletHeight, 0)), Quaternion.identity);
            }
            else Instantiate(fgb, transform.TransformPoint(new Vector3(-baseDistance, bulletHeight, 0)), Quaternion.Euler(0, 0, 180));
            cooldown = gun3Cooldown;
            gunExplosion.Play();
        }
    }
    
    //Code to throw a grenade
    private void ThrowGrenade(bool left)
    {
        if(cooldown2 <= 0)
        {
            if (!left)
            {
                //Spawn a grenade and set its velocity
                GameObject yeet = Instantiate(grenade, transform.TransformPoint(new Vector3(grenadeDistance, bulletHeight, 0)), Quaternion.identity);
                yeet.GetComponent<Rigidbody2D>().velocity = new Vector2(3, 3);
            }
            else
            {
                //Spawn a grenade and set its velocity
                GameObject yeet = Instantiate(grenade, transform.TransformPoint(new Vector3(-grenadeDistance, bulletHeight, 0)), Quaternion.identity);
                yeet.GetComponent<Rigidbody2D>().velocity = new Vector2(-3, 3);
            }
            cooldown2 = grenadeCooldown;
            manager.GrenadeThrown();
        }
    }
}
