using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LineController : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void IncreasePoints()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        var numPoints = lineRenderer.GetPositions(positions);

        Vector3[] newPositions = new Vector3[positions.Length * 2];
        int j = 0;
        for (int i = 0; i < positions.Length; ++i) {
            j = i * 2;

            var currI = i;
            var nextI = i + 1;
            if (i + 1 >= positions.Length) {
                nextI = (i + 1) - positions.Length;
            }

            var p1 = positions[currI];
            var p2 = positions[nextI];

            var averagePos = (p1 + p2) / 2;
            newPositions[j] = p1;
            newPositions[j + 1] = averagePos;
        }

        lineRenderer.positionCount = newPositions.Length;
        lineRenderer.SetPositions(newPositions);
    }

    public void DecreasePoints()
    {

    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(LineController))]
public class LineControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Increase points")) {
            (target as LineController).IncreasePoints();
        }
        if (GUILayout.Button("Decrease points")) {
            (target as LineController).DecreasePoints();
        }

        if (EditorGUI.EndChangeCheck()) {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif