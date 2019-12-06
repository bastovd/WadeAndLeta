using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraControls : MonoBehaviour
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
        }
        else {
            GlobeGround.transform.Rotate(Vector3.right, WalkingSpeed);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraControls))]
public class CameraControlsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("This controller rotates the ground as you walk forward.", MessageType.Info);
        if ((target as CameraControls).GlobeGround == null) {
            EditorGUILayout.HelpBox("Globe Ground must be assigned.", MessageType.Error);
        }

        base.OnInspectorGUI();
    }
}
#endif
