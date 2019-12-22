using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FloatAnimator : MonoBehaviour {
    public enum Transition {
        Ease,
        AnimationCurve
    }
    public Transition TransitionType = Transition.Ease;

    public float BeginValue = 0;
    public float EndValue = 0;
    [Space]
    public Ease EaseForward = Ease.Linear;
    public AnimationCurve CurveForward;
    public float DurationForward = 0; // instant
    public float DelayForward = 0;
    [Space]
    public Ease EaseBack = Ease.Linear;
    public AnimationCurve CurveBack;
    public float DurationBack = 0; // instant
    public float DelayBack = 0;
    [Space]
    public UnityEvent onBegin = null;
    public ExtendedUnityEvents.FloatUnityEvent onUpdate = null;
    public UnityEvent onComplete = null;

    private float value;

    private Tween tween;

    public void SetBeginValue(float val) {
        BeginValue = val;
    }
    public void SetEndValue(float val) {
        EndValue = val;
    }
    public void SetDurationForward(float val) {
        DurationForward = val;
    }
    public void SetDurationBack(float val) {
        DurationBack = val;
    }

    private void Awake() {
        value = BeginValue;
    }

    public void AnimateForward() {
        switch (TransitionType) {
            case Transition.Ease:
                tween = DOTween.To(() => value, x => value = x, EndValue, DurationForward).SetEase(EaseForward).SetDelay(DelayForward);
                break;
            case Transition.AnimationCurve:
                tween = DOTween.To(() => value, x => value = x, EndValue, DurationForward).SetEase(CurveForward).SetDelay(DelayForward);
                break;
        }

        tween.OnStart(OnBegin).OnUpdate(OnUpdate).OnComplete(OnComplete);
    }

    public void AnimateBack() {
        switch (TransitionType) {
            case Transition.Ease:
                tween = DOTween.To(() => value, x => value = x, BeginValue, DurationBack).SetEase(EaseBack).SetDelay(DelayBack);
                break;
            case Transition.AnimationCurve:
                tween = DOTween.To(() => value, x => value = x, BeginValue, DurationBack).SetEase(CurveBack).SetDelay(DelayBack);
                break;
        }

        tween.OnStart(OnBegin).OnUpdate(OnUpdate).OnComplete(OnComplete);
    }

    private void OnBegin() {
        onBegin?.Invoke();
    }

    private void OnUpdate() {
        onUpdate?.Invoke(value);
    }

    private void OnComplete() {
        onComplete?.Invoke();
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(FloatAnimator))]
public class FloatAnimatorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DEBUG");
        if (GUILayout.Button("Animate Forward")) {
            foreach (var t in targets) {
                (t as FloatAnimator).AnimateForward();
            }
        }
        if (GUILayout.Button("Animate Back")) {
            foreach (var t in targets) {
                (t as FloatAnimator).AnimateBack();
            }
        }
    }
}
#endif
