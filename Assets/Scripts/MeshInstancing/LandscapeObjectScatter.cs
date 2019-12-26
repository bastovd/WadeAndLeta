using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO : divide the landscape object rendering and the effects that can be applied
public class LandscapeObjectScatter : MonoBehaviour {
    public enum ScatterType {
        Circular,
        Spherical,
    };

    public Camera MainCamera;
    public Transform Root;
    [Header("Prefab to scatter")]
    public Mesh MeshToScatter;
    public Material MeshMaterial;
    [Header("Scatter params")]
    public ScatterType ScatterMode = ScatterType.Circular;
    public float RadiusMin = 2f;
    public float RadiusMax = 10f;
    public float AngleMin = 0f;
    public float AngleMax = 360f;
    public float ScaleMin = 1f;
    public float ScaleMax = 10f;
    public float Elevation = 0f;
    [Header("Spawning params")]
    public bool SpawnPeriodically = false;
    public float SpawnInterval = 5f; // seconds
    public float SpawnDuration = 15f; // seconds
    public bool SpawnRandom = false;
    public int SpawnCountMin = 0;
    public int SpawnCountMax = 100;
    public int Count = 100;
    [Header("Animate")]
    public bool Animate = false;
    public Ease AnimateEase = Ease.Linear;
    public bool AnimateRandomDuration = false;
    public float AnimateDurationMin = 2f;
    public float AnimateDurationMax = 2f;
    [Header("Materials params")]
    public float RenderDistance = 3f;
    public List<Texture2D> TexturesList;
    public List<Color> ColorsList;

    private bool scattered = false;
    private float beginTime = 0;

    [SerializeField]
    public class ScatterParams {
        public Vector3 position;
        public Quaternion rotation;
        public float scale;
        public Matrix4x4 mat;
        public MaterialPropertyBlock propertyBlock;

        public void Animate(float duration, float finalScale, Ease ease) {
            scale = 0f;
            DOTween.To(() => scale, x => scale = x, finalScale, duration).SetEase(ease);
        }
    }
    private List<ScatterParams> ScatterParamsList;

    private void Awake() {
        scattered = false;

        // scatter on awake for now
        if (ScatterParamsList == null) {
            // too slow to allocate here?
            ScatterParamsList = new List<ScatterParams>();
        } else {
            // cleate the existing list
            ScatterParamsList.Clear();
        }

        if (Root == null) {
            Root = transform;
        }

        Scatter();
    }

    private void Update() {
        if (scattered) {
            foreach (var i in ScatterParamsList) {
                if (Vector3.Distance(MainCamera.transform.position, i.position) > RenderDistance) continue;
                var mat = Matrix4x4.TRS(i.position, i.rotation, Vector3.one * i.scale);
                Graphics.DrawMesh(MeshToScatter, mat, MeshMaterial, 0, MainCamera, 0, i.propertyBlock);
            }
        }
    }

    public void Scatter(int num) {
        float radius = 0;
        float angle = 0;
        Vector3 pos = Root.position;

        MaterialPropertyBlock propertyBlock;

        for (int i = 0; i < Count; ++i) {
            radius = Random.Range(RadiusMin, RadiusMax);
            angle = Random.Range(Mathf.Deg2Rad * AngleMin, Mathf.Deg2Rad * AngleMax);

            if (ScatterMode == ScatterType.Circular) {
                // xz plane
                var s = MathExtensions.PointOnCircle(angle, radius);
                pos = new Vector3(s.x, Elevation, s.y);
            } else if (ScatterMode == ScatterType.Spherical) {
                var zAngle = Random.Range(0, Mathf.PI);
                var s = MathExtensions.PointOnSphere(angle, zAngle, radius);
                pos = s;
            }

            var scale = Random.Range(ScaleMin, ScaleMax);

            //mat = Matrix4x4.TRS(pos, Random.rotation, Vector3.one * scale);

            // random material properties
            propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", ColorsList[Random.Range(0, ColorsList.Count)]);
            propertyBlock.SetTexture("_MainTex", TexturesList[Random.Range(0, TexturesList.Count)]);

            ScatterParams item = new ScatterParams();
            item.position = pos;
            item.rotation = Random.rotation;
            item.scale = scale;

            //item.mat = mat;
            item.propertyBlock = propertyBlock;
            ScatterParamsList.Add(item);
            if (Animate) {
                var duration = AnimateDurationMin;
                if (AnimateRandomDuration) {
                    duration = Random.Range(AnimateDurationMin, AnimateDurationMax);
                }
                item.Animate(duration, scale, AnimateEase);
            }
        }

        // start drawing
        scattered = true;
    }

    public void Scatter() {
        if (SpawnRandom) {
            Scatter(Random.Range(SpawnCountMin, SpawnCountMax));
        } else {
            Scatter(Count);
        }

        if (SpawnPeriodically) {
            if (Time.time > SpawnDuration) return;
            StartCoroutine(WaitInterval());
        }
    }

    IEnumerator WaitInterval() {
        yield return new WaitForSeconds(SpawnInterval);
        Scatter();
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
