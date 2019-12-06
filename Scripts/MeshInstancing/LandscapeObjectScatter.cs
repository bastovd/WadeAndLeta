using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO : divide the landscape object rendering and the effects that can be applied
public class LandscapeObjectScatter : MonoBehaviour {
    public Camera MainCamera;
    [Header("Prefab to scatter")]
    public Mesh MeshToScatter;
    public Material MeshMaterial;
    [Header("Scatter params")]
    public float RadiusMin = 2f;
    public float RadiusMax = 10f;
    public float ScaleMin = 1f;
    public float ScaleMax = 10f;
    public float Elevation = 0f;
    public int Count = 100;
    [Header("Materials params")]
    public List<Texture2D> TexturesList;
    public List<Color> ColorsList;

    private bool scattered = false;

    private struct ScatterParams {
        public Matrix4x4 mat;
        public MaterialPropertyBlock propertyBlock;
    }
    private List<ScatterParams> ScatterParamsList;

    private void Awake() {
        scattered = false;

        // scatter on awake for now
        // DEBUG
        Scatter();
    }

    private void Update() {
        if (scattered) {
            foreach (var i in ScatterParamsList) {
                Graphics.DrawMesh(MeshToScatter, i.mat, MeshMaterial, 0, MainCamera, 0, i.propertyBlock);
            }
        }
    }

    public void Scatter() {
        if (ScatterParamsList == null) {
            // too slow to allocate here?
            ScatterParamsList = new List<ScatterParams>();
        } else {
            // cleate the existing list
            ScatterParamsList.Clear();
        }

        float radius = 0;
        float angle = 0;
        float PI2 = Mathf.PI * 2f;
        Vector3 pos = Vector3.zero;
        Matrix4x4 mat;

        MaterialPropertyBlock propertyBlock;
        
        for (int i = 0; i < Count; ++i) {
            radius = Random.Range(RadiusMin, RadiusMax);
            angle = Random.Range(0, PI2);
            // xz plane
            var x = radius * Mathf.Cos(angle);
            var z = radius * Mathf.Sin(angle);
            pos = new Vector3(x, Elevation, z);
            var scale = Random.Range(ScaleMin, ScaleMax);

            mat = Matrix4x4.TRS(pos, Random.rotation, Vector3.one * scale);

            // random material properties
            propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", ColorsList[Random.Range(0, ColorsList.Count)]);
            propertyBlock.SetTexture("_MainTex", TexturesList[Random.Range(0, TexturesList.Count)]);

            // DEBUG
            // Set the world pos and the camera pos
            //propertyBlock.SetVector("_WorldPos", pos);
            //propertyBlock.SetVector("_CameraPos", MainCamera.transform.position);
            //

            ScatterParams item = new ScatterParams();
            item.mat = mat;
            item.propertyBlock = propertyBlock;
            ScatterParamsList.Add(item);
        }

        // start drawing
        scattered = true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LandscapeObjectScatter))]
public class LandscapeObjectScatterEditor : Editor {
    private LandscapeObjectScatter _target;

    private void OnEnable() {
        _target = (LandscapeObjectScatter)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Scatter")) {
            _target.Scatter();
        }
    }
}
#endif
