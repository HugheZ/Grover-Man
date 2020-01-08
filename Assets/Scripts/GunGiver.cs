using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGiver : MonoBehaviour {
    public int gunIndex;    //which gun is it?
    public float maxHeight; //amplitude of sinusoid
    private float baseHeight;
    public List<Sprite> guns;   //list of all possible guns

    //gives sprite renderer the proper sprite to display and notes its starting height
    private void Start()
    {
        baseHeight = transform.position.y;
        GetComponent<SpriteRenderer>().sprite = guns[gunIndex];
    }

    //makes gun move up and down in a sinusoidal pattern while sitting in the level
    private void Update()
    {
        transform.position = new Vector2(transform.position.x, baseHeight + Height(Time.time));
    }

    //calculates change in height for the sinusoid
    private float Height(float y)
    {
        return Mathf.Sin(y) * maxHeight;
    }
}
