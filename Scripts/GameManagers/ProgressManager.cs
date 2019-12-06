using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    // this controller rotates the ground as you walk forward
    public Transform GlobeGround;
    public float WalkingSpeed = 0.02f;

    [Space]
    public bool WalkAutomatically = false;

    private void Update()
    {
        if (!WalkAutomatically) {
            if (Input.GetKey(KeyCode.W)) {
                GlobeGround.transform.Rotate(Vector3.right, WalkingSpeed);
            } else if (Input.GetKey(KeyCode.S)) {
                GlobeGround.transform.Rotate(Vector3.right, -WalkingSpeed);
            }
        } else {
            GlobeGround.transform.Rotate(Vector3.right, WalkingSpeed);
        }
    }
}
