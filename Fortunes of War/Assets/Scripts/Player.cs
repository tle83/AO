using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public double Id {
        get;
        set;
    }
    public string Username {
        get;
        set;
    }

    public double Level {
        get;
        set;
    }

    public Player (double id, string username, double level) {
        this.Id = id;
        this.Username = username;
        this.Level = level;
    }



}