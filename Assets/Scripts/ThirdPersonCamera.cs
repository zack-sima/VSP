using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    public Transform cameraPivot;
    public float offset; //how much away from player

    void Start() {
    }

    void FixedUpdate() {
        RaycastHit hit;
        transform.position = cameraPivot.position;
        transform.localRotation = Quaternion.identity;

        //Debug.DrawRay(player.transform.position, transform.TransformDirection(Vector3.back) * offset, Color.yellow);
        if (Physics.Raycast(cameraPivot.transform.position, transform.TransformDirection(Vector3.back), out hit, offset + 0.25f)) {
            //hit wall
            transform.position = hit.point;
            transform.Translate(Vector3.forward * 0.25f);
        } else {
            transform.Translate(Vector3.back * offset);
        }
    }
}