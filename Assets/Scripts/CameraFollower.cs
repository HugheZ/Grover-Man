using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

    public GameObject player; //player to follow
    public Vector3 offset;  //location offset
    public float deadDistance;  //dead zone distance
    float moveSpeedFast; //fast move speed
    float moveSpeedSlow; //slow move speed
    Vector3 velocity;   //velocity

    private void Start()
    {
        moveSpeedFast = 0;
        moveSpeedSlow = .5f;
    }

    // FixedUpdate is called when physics system moves
    void FixedUpdate()
    {
        //if there is a player to follow
        if (player)
        {
            //get the camera direction and set new position
            Vector3 newPos;
            float usedSpeed;
            Vector2 dirr = (Vector2)transform.position - (Vector2) player.transform.position;
            float distance = dirr.magnitude;
            //if outside dead zone, move the camera and set used movespeed
            if (distance > deadDistance)
            {
                newPos = player.transform.position + ((Vector3)dirr.normalized * deadDistance) + offset;
                usedSpeed = moveSpeedFast;
            }//else, get the new position and set to slow movespeed
            else
            {
                newPos = player.transform.position + offset;
                usedSpeed = moveSpeedSlow;
            }
            //smooth damp to replace camera position
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, usedSpeed);
        }
    }
}
