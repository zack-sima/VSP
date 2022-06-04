using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public InputField nameInput;
    void Start() {
        Application.targetFrameRate = 60;
        nameInput.text = PlayerPrefs.GetString("playerName");
    }
    public void LoadGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    void Update() {
        if (nameInput.text != "") {
            PlayerPrefs.SetString("playerName", nameInput.text);
        } else {
            PlayerPrefs.SetString("playerName", "Player");
        }

    }
}