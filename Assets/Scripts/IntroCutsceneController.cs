using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCutsceneController : MonoBehaviour {

    public GameObject finalActor;   //the final actor to trigger load UI
    public Text authors;    //authors text
    public Image splash;    //splash screen
    public Image pressStartPanel;   //press start panel
	
	// Update is called once per frame
	void Update () {
        //load UI when last actor dies
		if(finalActor == null)
        {
            pressStartPanel.gameObject.SetActive(true);
            authors.enabled = false;
            splash.enabled = true;
            this.enabled = false;
        }
	}
}
