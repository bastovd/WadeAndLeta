using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectPositionAnimatable : MonoBehaviour {
    public UnityEvent Trigger;

    private Vector3 initPos;
    private void Awake() {
        initPos = transform.localPosition;   
    }

    public void SetX(float v) {
        if (initPos == null) {
            initPos = transform.localPosition;
        }
        transform.localPosition = new Vector3(v, initPos.y, initPos.z);
    }

    public void SetY(float v) {
        if (initPos == null) {
            initPos = transform.localPosition;
        }
        transform.localPosition = new Vector3(initPos.x, v, initPos.z);
    }

    public void SetZ(float v) {
        if (initPos == null) {
            initPos = transform.localPosition;
        }
        transform.localPosition = new Vector3(initPos.x, initPos.y, v);
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectPositionAnimatable))]
public class ObjectPositionAnimatableEditor : Editor {
    ObjectPositionAnimatable _target;

    private void OnEnable() {
        _target = (ObjectPositionAnimatable)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DEBUG");
        if (GUILayout.Button("Trigger Pos")) {
            _target.Trigger?.Invoke();
        }
    }
}
#endif
