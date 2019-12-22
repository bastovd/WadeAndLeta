using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PerformanceTest : MonoBehaviour
{
    [Header("Draw Gameobject params")]
    public GameObject Original;
    [Header("Draw Mesh params")]
    public Mesh mesh;
    public Material material;
    public Camera cameraToDraw;
    [Header("Common")]
    public float PosZ = 0f;
    public enum PerformanceTestType {
        DrawMesh,
        DrawGameObject
    }
    public PerformanceTestType TestType = PerformanceTestType.DrawMesh;
    public int NumObjects = 10;
    public float SpacingX = 0.1f;
    public float SpacingY = 0.1f;

    [HideInInspector] public List<Vector3> positions;
    // gameobjects test
    [HideInInspector] public List<GameObject> objects;

    private void Awake() {
        CreateObjects();
    }

    public void CreateObjects() {
        GeneratePositions();
        switch (TestType) {
            case PerformanceTestType.DrawGameObject:
                TestGameObjects();
                break;
            case PerformanceTestType.DrawMesh:
                TestMeshes();
                break;
        };
    }

    private void GeneratePositions() {
        if (positions == null) positions = new List<Vector3>();
        else positions.Clear();

        // put them in a grid
        int cols = (int)Mathf.Sqrt(NumObjects);
        float minX = -cols / 2f * SpacingX;
        float minY = -cols / 2f * SpacingY;
        int currCol = 0;
        for (int i = 0; i < NumObjects; ++i) {
            var pos = Vector3.zero;
            pos.x = minX + currCol * SpacingX;
            pos.y = minY;
            pos.z = PosZ;
            positions.Add(pos);

            currCol++;
            if (currCol == cols) {
                currCol = 0;
                minY += SpacingY;
            }
        }
    }

    private void TestGameObjects() {
        if (objects == null) objects = new List<GameObject>();
        else objects.Clear();

        foreach (Transform t in transform) {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < NumObjects; ++i) {
            GameObject o = GameObject.Instantiate(Original, positions[i], Quaternion.identity, transform);
            objects.Add(o);
        }
    }

    private void LateUpdate() {
        if (TestType == PerformanceTestType.DrawMesh) {
            TestMeshes();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight) ||
            OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickRight)) {
            NumObjects += 10;
            if (TestType == PerformanceTestType.DrawGameObject) {
                CreateObjects();
            }
        } else if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft) ||
            OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickLeft)) {
            NumObjects -= 10;
            if (TestType == PerformanceTestType.DrawGameObject) {
                CreateObjects();
            }
        }
    }

    private void TestMeshes() {
        if (positions == null || positions.Count != NumObjects) GeneratePositions();
        for (int i = 0; i < NumObjects; ++i) {
            Graphics.DrawMesh(mesh, positions[i], Quaternion.identity, material, 0, cameraToDraw);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PerformanceTest))]
public class PerformanceTestEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test Objects")) {
            (target as PerformanceTest).CreateObjects();
        }
    }
}
#endif
