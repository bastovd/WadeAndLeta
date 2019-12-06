using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DirectorPlayback : MonoBehaviour
{
    [System.Serializable]
    public struct Duration
    {
        public int Frames;
        public int Seconds;
    }
    public static Duration TotalDuration;
    public int DurationSeconds = 0;

    [Header("Sky Colors")]
    public Camera MainCamera;
    public float SkyColorTransitionDuration = 6f;
    public List<Color> SkyColors;
    private int currColor;

    private PlayableDirector director {
        get {
            return GetComponent<PlayableDirector>();
        }
    }

    private void Awake()
    {
        //director.Play();   
        currColor = 0;
        TransitionSkyColors(SkyColors[1], 20);
        TransitionSkyColors(SkyColors[2], 40);
    }

    public void Play()
    {
        director.Play();
    }

    // Debug that just looks at the index
    public void TransitionSkyColors()
    {
        currColor++;
        if (currColor >= SkyColors.Count) {
            currColor = 0;
        }
        TransitionSkyColors(SkyColors[currColor], 0);
    }

    public void TransitionSkyColors(Color toColor, float delay) {
        MainCamera.DOColor(toColor, SkyColorTransitionDuration)
            .SetDelay(delay)
            .SetEase(Ease.Linear);
    }

    private void OnValidate()
    {
        TotalDuration.Seconds = DurationSeconds;
        TotalDuration.Frames = DurationSeconds * 60;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DirectorPlayback))]
public class DirectorPlaybackEditor : Editor
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

        if (GUILayout.Button("Play")) {
            (target as DirectorPlayback).Play();
        }
        if (GUILayout.Button("Transition Sky Color")) {
            (target as DirectorPlayback).TransitionSkyColors();
        }
    }
}
#endif