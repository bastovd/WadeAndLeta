using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectAnimatorBounce : ObjectAnimator
{
    [Space]
    [Header("Bounce Params")]
    public float Amplitude = 0;
    public bool RandomizeAmplitude = true;
    public float MinAmplitude = 0f;
    public float MaxAmplitude = .2f;
    [Space]
    public float TimeOffset = 0;
    public bool RandomizeTimeOffset = true;
    public float MinTimeOffset = 0f;
    public float MaxTimeOffset = 1f;

    protected override void Awake()
    {
        base.Awake();

        if (RandomizeAmplitude) {
            Amplitude = Random.Range(MinAmplitude, MaxAmplitude);
        }
        if (RandomizeTimeOffset) {
            TimeOffset = Random.Range(MinTimeOffset, MaxTimeOffset);
        }

        Play();
    }

    protected override void Update()
    {
        base.Update();
        if (isPlaying) {
            var pos = transform.localPosition;
            var initPos = InitTransform.localPosition;
            // xy
            float y = initPos.y + Amplitude * Mathf.Cos(Time.time + TimeOffset);
            transform.localPosition = new Vector3(pos.x, y, pos.z);
        }
    }

    public override void Play()
    {
        base.Play();
        isPlaying = true;
    }

    public override void Stop()
    {
        base.Stop();
        isPlaying = false;
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectAnimatorBounce))]
public class ObjectAnimatorBounceEditor : Editor
{
    private void OnEnable()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (var t in targets) {
            if ((t as ObjectAnimatorBounce).RandomizeAmplitude) {
                //EditorGUILayout.MinMaxSlider()
            }
        }
    }
}
#endif
