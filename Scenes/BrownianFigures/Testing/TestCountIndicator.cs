using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestCountIndicator : MonoBehaviour
{
    public TMP_Text TMProText;
    public TMP_Text TMProTextFPS;
    public PerformanceTest PTestRef;

    float deltaTime = 0.0f;
    int cachedCount = 0;

    void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        TMProTextFPS.SetText((1.0f / deltaTime).ToString());
        if (cachedCount != PTestRef.NumObjects) {
            cachedCount = PTestRef.NumObjects;
            TMProText.SetText(cachedCount.ToString());
        }
    }
}
