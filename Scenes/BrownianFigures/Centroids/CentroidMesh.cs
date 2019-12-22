using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class CentroidMesh : MonoBehaviour {
    [HideInInspector] public Mesh ConstructedMesh;
    private float radius;
    public float Radius;
    private int numVertices;
    public int NumVertices;

    private void Awake() {
        ConstructedMesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = ConstructedMesh;
    }

    void PopulateMesh() {
        var center = new Vector3(0, 0, 0);
        var angle = Mathf.PI * 2f / NumVertices;
        List<Vector3> verts = new List<Vector3>();
        verts.Add(center);
        for (int i = 0; i < NumVertices; ++i) {
            var pointOnCircle = MathExtensions.PointOnCircle(i * angle, Radius);
            verts.Add(new Vector3(pointOnCircle.x, pointOnCircle.y));
        }
        ConstructedMesh.SetVertices(verts);

        List<int> tris = new List<int>();
        for (int i = 1; i <= NumVertices; ++i) {
            tris.Add(0);
            tris.Add(i);
            if (i == NumVertices) {
                tris.Add(1);
            } else {
                tris.Add(i + 1);
            }
        }
        ConstructedMesh.SetTriangles(tris, 0);

        ConstructedMesh.MarkDynamic();
        ConstructedMesh.RecalculateBounds();
        ConstructedMesh.RecalculateNormals();
        ConstructedMesh.RecalculateTangents();
    }

    private void OnValidate() {
        if (radius != Radius || numVertices != NumVertices) {
            radius = Radius;
            numVertices = NumVertices;
            PopulateMesh(); 
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CentroidMesh))]
public class CentroidMeshEditor : Editor {

    private void OnEnable() {
        
    }

    private void OnDestroy() {
        
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
#endif
