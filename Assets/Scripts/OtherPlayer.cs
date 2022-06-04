using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : MonoBehaviour {
    public Transform canvas;
    public Text nameDisplay;

    public void SetName(string newName) {
        nameDisplay.text = newName;
    }
    void Update() {
        canvas.rotation = Camera.main.transform.rotation;
    }
}