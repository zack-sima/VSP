using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
    public static Controller instance;
    public static List<Door> doors;
    public Text doorText;

    void Awake() {
        doors = new List<Door>();
    }
    void Start() {
        instance = this;
    }

    void Update() {
        bool noDoors = true;
        foreach (Door d in doors) {
            if (d.nearPlayer) {
                noDoors = false;
                break;
            }
        }
        if (noDoors) {
            doorText.enabled = false;
        } else {
            doorText.enabled = true;
        }
    }
}