using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotator : MonoBehaviour
{
    public Transform Root;
    public Transform SpawnPoint;
    public float Radius = 6f;
    public float Angle = 15f;
    [Space]
    public List<GameObject> ToRotate;

    private GameObject viewCamera;
    public GameObject ViewCamera {
        get {
            if (viewCamera == null) {
                viewCamera = GameObject.Find("CenterEyeAnchor");
            }
            return viewCamera;
        }
    }

    private void Start()
    {
        StartCoroutine(CenterWorldDelayed());
    }

    public void CenterWorld()
    {
        if (ViewCamera == null) return;
        var direction = ViewCamera.transform.forward;
        var angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        foreach (var r in ToRotate) {
            r.transform.Rotate(Vector3.up, angle);
        }
    }

    public void PlaceCamera()
    {
        // position on yz
        float x = 0;
        float y = Radius * Mathf.Cos(Angle * Mathf.Deg2Rad);
        float z = Radius * Mathf.Sin(Angle * Mathf.Deg2Rad);
        // relative to world origin for now
        SpawnPoint.localPosition = new Vector3(x, y, z);
        Root.position = SpawnPoint.position;
        // some funky rotation here
        Root.eulerAngles = new Vector3(0, 180, 0);

        // destroy the spawn point since we are alreadt placed
        Destroy(SpawnPoint.gameObject);
    }

    IEnumerator CenterWorldDelayed()
    {
        yield return new WaitUntil(() => {
            return ViewCamera.transform.rotation != Quaternion.identity;
        });

        CenterWorld();
        PlaceCamera();
    }
}
