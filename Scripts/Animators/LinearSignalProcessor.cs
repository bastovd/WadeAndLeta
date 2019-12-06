using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinearSignalProcessor : MonoBehaviour
{
    /// Signal can come from an audio file.
    /// In this class, just simulate a random audio signal
    /// -------------------------
    /// Can implement a circular list, but sounds like an overkill.
    /// Right now just make it into an even number array
    /// [amp, wait, amp, wait, amp, wait, ...]

    public bool Loop = true;
    [Header("DEBUG: Generator options.")]
    public bool DebugGenerate = true;
    public int NumIntervals = 50;
    public float MinAmplitude = 0f;
    public float MaxAmplitude = 0f;
    public float MinWait = 0f;
    public float MaxWait = 0f;
    [Space]
    public ExtendedUnityEvents.FloatUnityEvent BeforeSignal = null;
    public ExtendedUnityEvents.FloatUnityEvent AfterSignal = null;
    public ExtendedUnityEvents.FloatUnityEvent WaitChanged = null;
    public ExtendedUnityEvents.FloatUnityEvent AmplitudeChanged = null;

    private struct SignalTick {
        public float amplitude;
        public float wait;
    }

    private Queue<SignalTick> signal;
    private int currIndex = 0;

    private void Awake() {
        if (DebugGenerate) GenerateDebugSignal();

        Step();
    }

    void GenerateDebugSignal() {
        signal = new Queue<SignalTick>();

        for (int i = 0; i < NumIntervals; i ++) {
            var tick = new SignalTick();
            tick.amplitude = Random.Range(MinAmplitude, MaxAmplitude);
            tick.wait = Random.Range(MinWait, MaxWait);

            signal.Enqueue(tick);
        }
    }

    public void Step() {
        if (signal.Count > 0) {
            var tick = signal.Dequeue();
            WaitChanged?.Invoke(tick.wait);
            AmplitudeChanged?.Invoke(tick.amplitude);

            StartCoroutine(wait(tick));

            if (Loop) {
                signal.Enqueue(tick);
            }
        }
    }

    IEnumerator wait(SignalTick t) {
        BeforeSignal?.Invoke(t.amplitude);
        yield return new WaitForSeconds(t.wait);
        AfterSignal?.Invoke(t.amplitude);
        Step();
    }
}
