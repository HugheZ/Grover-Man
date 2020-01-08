using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlFader : MonoBehaviour {

    float startTime;    //start time
    public float fadeTime;  //time to fade
    bool totallyFaded;  //is totally faded
    bool left;  //is left
    public Image godAura;   //the aura image
    public Text instructions;   //instruction text
    public Image instructionBox;    //instruction panel
    public Animator cutsceneAnim;   //cutscene animations

	// Use this for initialization
	void Start () {
        totallyFaded = false;
        left = false;
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        //while not faded totally, fade in aura
        if (!totallyFaded)
        {
            float totalFade = Time.time - startTime;
            godAura.color = new Color(godAura.color.r, godAura.color.g, godAura.color.b, 1 * (totalFade/fadeTime));
            if (totalFade >= 1) totallyFaded = true;
        }
        //if the player hasn't left, enable the instruction textbox
        else if(!left)
        {
            left = true;
            instructions.enabled = true;
            instructionBox.enabled = true;
        }
        //if submit is hit, disable the text and set the trigger to leave
        if (Input.GetButtonDown("Submit") && instructions.enabled)
        {
            instructions.enabled = false;
            instructionBox.enabled = false;
            cutsceneAnim.SetTrigger("Leave");
        }
	}
}
