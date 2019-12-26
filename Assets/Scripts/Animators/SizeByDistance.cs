using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SizeByDistance : MonoBehaviour {
    public Transform Target;
    [Header("Scale Params")]
    public float BeginSize = 1f;
    public float Size = 1f;
    public float Distance = 1f;
    [Space]
    public UnityEvent OnBeforeFront = null;
    public UnityEvent OnFrontTarget = null;
    public UnityEvent OnExactTarget = null;
    public UnityEvent OnBackTarget = null;
    public UnityEvent OnAfterBack = null;

    private enum Proximity {
        BeforeFirstHit,
        FirstHit,
        ExactHit,
        LastHit,
        AfterLastHit
    }
    private Proximity proximity;
    private float eps = 0.001f;
    private float diffSize = 0;

    private void Awake() {
        transform.localScale = Vector3.one * BeginSize;
        diffSize = Size - BeginSize;

        proximity = Proximity.BeforeFirstHit;
        OnBeforeFront?.Invoke();
    }

    private void Update() {
        var distance = Vector3.Distance(transform.position, Target.position);
        var distanceRatio = 1f - distance / Distance;
        var size = (diffSize * distanceRatio) + BeginSize;
        
        if (proximity == Proximity.BeforeFirstHit) {
            if (distance <= Distance) {
                OnFrontTarget?.Invoke();
                proximity = Proximity.FirstHit;
            }
        } else if (proximity == Proximity.FirstHit) {
            if (distance <= eps) {
                OnExactTarget?.Invoke();
                proximity = Proximity.ExactHit;
            }
        } else if (proximity == Proximity.ExactHit) {
            if (distance >= Distance) {
                OnBackTarget?.Invoke();
                proximity = Proximity.LastHit;
            }
        } else if (proximity == Proximity.LastHit) {
            OnAfterBack?.Invoke();
            proximity = Proximity.AfterLastHit;
        } else if (proximity == Proximity.AfterLastHit) {
            //
        }

        if (distance < Distance) {
            transform.localScale = size * Vector3.one;
        } else {
            //transform.localScale = BeginSize * Vector3.one;
        }
    }
}
