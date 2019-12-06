using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppablePool : MonoBehaviour
{
    public List<Poppable> PoppablesList;

    public Queue<Poppable> AvailablePoppables;

    public static PoppablePool Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else DestroyImmediate(this);

        AvailablePoppables = new Queue<Poppable>();
        foreach (var p in PoppablesList) {
            p.gameObject.SetActive(false);
            AvailablePoppables.Enqueue(p);
        }
    }

    public Poppable GetPoppable()
    {
        // what to do if none available?
        if (AvailablePoppables.Count <= 0) return null;

        var p = AvailablePoppables.Dequeue();
        p.gameObject.SetActive(true);

        return p;
    }

    public void ReturnPoppable(Poppable p)
    {
        p.gameObject.SetActive(false);
        AvailablePoppables.Enqueue(p);
    }
}
