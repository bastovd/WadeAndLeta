using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FogAnimatable : MonoBehaviour
{
    public UnityEvent Trigger;

    public void SetFogDensity(float v) {
        RenderSettings.fogDensity = v;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FogAnimatable))]
public class FogAnimatableEditor : Editor {
    FogAnimatable _target;

    private void OnEnable() {
        _target = (FogAnimatable)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DEBUG");
        if (GUILayout.Button("Trigger Fog")) {
            _target.Trigger?.Invoke();
        }
    }
}
#endif
