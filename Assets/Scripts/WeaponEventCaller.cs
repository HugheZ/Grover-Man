using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEventCaller : MonoBehaviour {

    public GameObject missile;  //missile prefab
    public Transform spawner;   //generic spawner location
    public List<GameObject> lasers; //list for laser object poool
    public List<GameObject> cores;  //list for laser core object pool
    int lastSpawned;    //last spawned value, used for object pooling
    public GameObject explosion;    //explosion used for last phase
    public AudioClip weaponClip;    //generic weapon clip to be used by attack

    //Called by animators, gets random variable for next cycle
    public void GetCycleFloat()
    {
        GetComponent<Animator>().SetFloat("ATTACK_TYPE", Random.Range(0, 100));
    }

    //Calls end cycle in parent to update animations
    public void EndCycle()
    {
        GetComponentInParent<MarxController>().EndedCycle();
    }

    //Plays the generic weapon audio clip
    public void PlayWeaponSound()
    {
        GetComponent<AudioSource>().PlayOneShot(weaponClip);
    }

    //Launches a missile
    public void LaunchMissile()
    {
        Instantiate(missile, spawner.position, Quaternion.identity);
    }

    //Spawns an explosion at the given spawner location
    public void Explode()
    {
        Instantiate(explosion, spawner.position, Quaternion.identity);
    }

    //Enables a random laser at a random position, looped until found one not active
    public void EnableRandomLaser()
    {
        //base random index
        int chosen = Random.Range(0, lasers.Count);
        //how many times have we seen the start, if 2 break
        int fullLoop = 0;

        //loop through to find laser to spawn
        for(int i = chosen; fullLoop < 2; i = (i + 1) % lasers.Count)
        {
            //if at first, increment full loop count
            if (i == chosen) fullLoop++;
            //if laser is available, set active and return
            if (!lasers[i].activeInHierarchy)
            {
                lasers[i].SetActive(true);
                return;
            }
        }
    }

    //Spawns a core from an object pool of cores
    public void SpawnCore()
    {
        //loop through circular list for core, starting at lastspawned+1
        for(int i = (lastSpawned + 1) % cores.Count; i != lastSpawned; i= (i + 1) % cores.Count)
        {
            //if core isn't active, activate it, update last spawned, return
            if (!cores[i].activeInHierarchy)
            {
                cores[i].transform.position = spawner.position;
                cores[i].SetActive(true);
                lastSpawned = i;
                return;
            }
        }
    }
}
