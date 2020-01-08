using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFacilitator : MonoBehaviour {

    public int stage; //stage of the boss
    private static BossFacilitator _instance = null; //singleton instance


    //return instance of the level manager
    public static BossFacilitator Instance
    {
        get
        {
            return _instance;
        }
    }

    //awake, checks for instance and destroys the new manager if multiple are inputted
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
