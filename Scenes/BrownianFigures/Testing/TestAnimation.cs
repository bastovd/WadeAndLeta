using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Oculus;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestAnimation : MonoBehaviour
{
    public PerformanceTest PTest;
    public float Duration = 4f;
    public float Up = 1f;
    private float up = 0;

    private void Awake() {
        up = Up;
    }

    void AnimateGameObjects() {
        for (int i = 0; i < PTest.NumObjects; ++i) {
            PTest.objects[i].transform.DOLocalMoveY(PTest.positions[i].y + up, Duration).SetEase(Ease.InOutQuart);
        }
    }

    void AnimateMeshes() {
        for (int i = 0; i < PTest.NumObjects; ++i) {
            DOTween.To(() => PTest.positions[i], x => PTest.positions[i] = x, PTest.positions[i] + Vector3.up * up, Duration).SetEase(Ease.InOutQuart);
        }
    }

    public void Animate() {
        if (PTest.TestType == PerformanceTest.PerformanceTestType.DrawGameObject) {
            AnimateGameObjects();
        } else {
            AnimateMeshes();
        }
    }

    private void Update() {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown) ||
                 OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown)) {
            up = -1f * Up;
            Animate();
        }
        else if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp) ||
                 OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickUp)) {
            up = 1f * Up;
            Animate();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestAnimation))]
public class TestAnimationEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Animate")) {
            (target as TestAnimation).Animate();
        }
    }
}
#endif
