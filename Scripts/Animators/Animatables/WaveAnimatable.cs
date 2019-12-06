using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaveAnimatable : MonoBehaviour
{
    public UnityEvent Trigger;

    public Transform Root;

    List<Renderer> renderers;

    private void Awake() {
        if (Root == null) return;
        renderers = new List<Renderer>();

        foreach (Transform child in Root) {
            renderers.Add(child.GetComponent<Renderer>());
        }
    }

    public void SetWaveHeight(float v) {
        // material is shared between gameobjects, so maybe this works?
        foreach (var r in renderers) {
            r.material.SetFloat("_Amount", v);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WaveAnimatable))]
public class WaveAnimatableEditor : Editor {
    WaveAnimatable _target;

    private void OnEnable() {
        _target = (WaveAnimatable)target;
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
