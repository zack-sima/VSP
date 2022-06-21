using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    public Door otherDoor;
    public bool nearPlayer;

    void Start() {
        Controller.doors.Add(this);
    }

    void Update() {
        //if walking from outside teleport
        if (Vector3.Distance(transform.position, SC_FPSController.instance.transform.position) < 2) {
            nearPlayer = true;
            if (Input.GetKeyDown(KeyCode.E) && !otherDoor.nearPlayer) {
                SC_FPSController.instance.SetPosition(otherDoor.transform.position);
            }
        } else {
            nearPlayer = false;
        }
    }
}