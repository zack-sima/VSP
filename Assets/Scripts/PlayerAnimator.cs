using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    Animator animator;
    public float horizontalMove, verticalMove, animSpeed;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        animator.SetFloat("Vertical", verticalMove, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", horizontalMove, 0.1f, Time.deltaTime);

        animator.SetFloat("WalkSpeed", animSpeed);
    }
}