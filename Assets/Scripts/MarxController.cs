using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarxController : MonoBehaviour {

    public BoxCollider2D weakPoint; //weakpoint hitbox
    public CapsuleCollider2D hitBox; //general hitbox
    public float vulnerableTime; //time to be vulnerable
    public int maxCycles; //number of cycles to go through before overheating
    private int currentCycles; //current cycles at
    public bool weak; //is it open for damage
    public bool dead; //is dead
    public int health; //health remaining
    public int stage; //stage of boss, 1 2 or 3
    public Animator anim; //animator
    private BossFacilitator stageGiver; //persistent facilitator used to set stage
    
    //HANDS PHASE
    public GameObject hands; //hands holder object
    public GameObject handsForeground; //play area for phase 1
    public GameObject handsBackground; //background for phase 1
    //MISSILES PHASE
    public GameObject missiles; //missiles holder
    public GameObject missilesForeground; //play area for phase 2
    public GameObject missilesBackground; //background for phase 2
    //LASERS PHASE
    public GameObject lasers; //lasers holder
    public GameObject lasersForeground; //play area for phase 3
    public GameObject lasersBackground; //background for phase 3

    private GameObject weapon; //weapon used
    public ParticleSystem pop; //weak pop effect

    //Random Generator for audio
    System.Random random;

    //Audio
    public AudioClip shieldBreak; //Sound when the shield breaks and Mecha Marx is open to damage
    public AudioClip startingQuote; //What MechaMarx will say when the fight begins
    public List<AudioClip> quotes; //All the quotes MechaMarx will say during the fight
    public AudioClip finalQuote; //What MechaMarx will say when he finally dies

    // Use this for initialization
    void Start () {
        //get stage from facilitator
        random = new System.Random();
        stageGiver = FindObjectOfType<BossFacilitator>();
        stage = stageGiver.stage;
        //get active weapon from stage, activate it
        switch (stage) {
            case 3:
                weapon = hands;
                handsForeground.SetActive(true);
                handsBackground.SetActive(true);
                GetComponent<AudioSource>().PlayOneShot(startingQuote); //Play the starting quote
                break;
            case 2:
                weapon = missiles;
                missilesForeground.SetActive(true);
                missilesBackground.SetActive(true);
                GetComponent<AudioSource>().PlayOneShot(quotes[random.Next(0, 3)]); //Play a quote at random
                break;
            case 1:
                weapon = lasers;
                lasersForeground.SetActive(true);
                lasersBackground.SetActive(true);
                GetComponent<AudioSource>().PlayOneShot(quotes[random.Next(0, 3)]); //Play a quote at random
                break;
            default: print("There is no behavior for this stage");
                break;
        }
        weapon.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    //Toggles weakness of the boss to true
    public void OpenForDamage()
    {
        //set weak state
        hitBox.enabled = false;
        weakPoint.enabled = true;
        weak = true;
        anim.SetBool("WEAK", weak);
        weapon.GetComponent<Animator>().SetBool("WEAK", weak);
        //play particle effect
        pop.Play();
        //play sound
        GetComponent<AudioSource>().PlayOneShot(shieldBreak);
    }

    //Toggles weakness of the boss to false
    public void CloseForDamage()
    {
        //unset weak state
        hitBox.enabled = true;
        weakPoint.enabled = false;
        weak = false;
        anim.SetBool("WEAK", weak);
        weapon.GetComponent<Animator>().SetBool("WEAK", weak);
    }

    //Handles death of the boss
    private void Die()
    {
        //decrement stage, set die flag
        stageGiver.stage--;
        dead = true;
        anim.SetBool("DEAD", dead);

        //If not the final stage on death, play sounds
        if (stage != 1)
        {
            GetComponent<AudioSource>().PlayOneShot(quotes[random.Next(0, 3)]); //Play a quote at random
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(finalQuote); //Play the final quote if this is the last time Marx is brought down
        }

        weapon.GetComponent<Animator>().SetBool("DEAD", true);
    }

    //Handles collisions, if hit by bullet and weak set state, guaranteed to only be hit on weak point
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if open for damage and hitting weak point
        if (weak && collision.otherCollider.Equals(weakPoint))
        {
            //if hit by a bullet
            if(collision.gameObject.tag == "PlayerBullet")
            {
                //lose health, play sound, and die if need be
                health--;
                print("Ouch, Grover Man hath hit the mighty lord of red");
                //TODO: play hurt sound effect
                if(health == 0)
                {
                    Die();
                }
            }
        }
    }

    //Called by animators, pings the boss to allow for opening for damge
    public void EndedCycle()
    {
        //decrement cycle, open for damage if needed
        currentCycles++;
        if(currentCycles > maxCycles)
        {
            currentCycles = 0;
            OpenForDamage();
        }
    }
}
