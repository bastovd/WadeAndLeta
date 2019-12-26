using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProgressMountain : MonoBehaviour
{
    public float GrowRate = 0.2f;
    private int numPopped = 0;

    private void Awake()
    {
        numPopped = 0;
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.EVENT_POPPED, HandlePopped);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.EVENT_POPPED, HandlePopped);
    }

    void HandlePopped()
    {
        numPopped++;
        // DEBUG
        // just scale for now
        transform.localScale = new Vector3(5, numPopped * GrowRate, 5);
    }

    // API
    public int NumPopped
    {
        get {
            return numPopped;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ProgressMountain))]
public class ProgressMountainEditor : Editor
{
    ProgressMountain _target;

    private void OnEnable()
    {
        _target = (ProgressMountain)target;
    }

    private void OnDestroy()
    {
        //
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Num Popped", _target.NumPopped.ToString());

        // every frame?
        EditorUtility.SetDirty(_target);
    }

    private void OnSceneGUI()
    {
        var p1 = _target.transform.position;
        // put to the ground
        p1.y = 0;

        var num = _target.NumPopped;
        var p2 = p1;
        p2.y = num * _target.GrowRate;

        Handles.DrawLine(p1, p2);
    }
}
#endif
