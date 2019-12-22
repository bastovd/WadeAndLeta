using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectSizeAnimatable : MonoBehaviour {
    public UnityEvent Trigger;

    public void SetScale(float v) {
        transform.localScale = new Vector3(v, v, v);
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectSizeAnimatable))]
public class ObjectSizeAnimatableEditor : Editor {
    ObjectSizeAnimatable _target;

    private void OnEnable() {
        _target = (ObjectSizeAnimatable)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DEBUG");
        if (GUILayout.Button("Trigger Wave")) {
            _target.Trigger?.Invoke();
        }
    }
}
#endif
