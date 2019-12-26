using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoppablesManager : MonoBehaviour
{
    public Transform World;
    public Transform Storage;
    [Header("Spawning Placement")]
    public float Radius = 1f;
    public float SpawnAngle = 0;
    public float Amplitude = 0.1f;
    public float HorizontalVariation = 0.1f;
    [Header("Spawning Timing")]
    public float TimeIntervalSeconds = 10f;
    public float TimeIntervalVariation = 2f;

    public static PoppablesManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else DestroyImmediate(this);

        Place();
    }

    public void PlacePoppable()
    {
        var poppable = PoppablePool.Instance.GetPoppable();
        if (poppable == null) return;

        float x = Random.Range(-HorizontalVariation, HorizontalVariation);
        var radius = Radius + Random.Range(-Amplitude, Amplitude);
        float y = radius * Mathf.Cos(SpawnAngle * Mathf.Deg2Rad);
        float z = radius * Mathf.Sin(SpawnAngle * Mathf.Deg2Rad);
        poppable.transform.localPosition = new Vector3(x, y, z);

        // place in the yz plane with some vertical and horizontal variation
        poppable.transform.SetParent(World, true);

        // call to handle any init functions
        poppable.OnPlaced();
    }

    public void ReturnPoppable(Poppable p)
    {
        p.transform.SetParent(Storage, false);
        p.transform.localPosition = Vector3.zero;
        p.transform.localRotation = Quaternion.identity;
        //p.transform.localScale = Vector3.one;
        PoppablePool.Instance.ReturnPoppable(p);
    }

    public void Place()
    {
        StopCoroutine(PlaceAndWait(0));
        var t = Random.Range(TimeIntervalSeconds - TimeIntervalVariation, TimeIntervalSeconds + TimeIntervalVariation);
        StartCoroutine(PlaceAndWait(t));
    }

    IEnumerator PlaceAndWait(float t)
    {
        PlacePoppable();
        yield return new WaitForSeconds(t);
        Place();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PoppablesManager))]
public class PoppablesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Release a pop")) {
            (target as PoppablesManager).PlacePoppable();
        }
    }
}
#endif


