using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnimator : MonoBehaviour
{
    public Transform InitTransform;

    public Transform FinalTransform;

    protected bool isPlaying = false;

    protected virtual void Awake() {
        if (InitTransform == null) {
            InitTransform = transform;
        }
    }

    protected virtual void Update() {}

    public virtual void Play() {}

    public virtual void Stop() {}
}
