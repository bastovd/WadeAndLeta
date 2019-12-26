using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Poppable : MonoBehaviour
{
    public float PopDistanceScale = 0.8f;
    [Space]
    public float SizeRangeMin = 0.2f;
    public float SizeRangeMax = 0.4f;
    public bool RandomSize = true;
    public float Size = 0.1f;

    private Collider thisCollider;
    private Vector3 center;
    private float popDistance = 0;

    // property block
    private Renderer _renderer {
        get {
            return GetComponent<Renderer>();
        }
    }
    private MaterialPropertyBlock _cachedBlock;
    private MaterialPropertyBlock _block {
        get {
            if (_cachedBlock == null) _cachedBlock = new MaterialPropertyBlock();
            return _cachedBlock;
        }
        set {
            _cachedBlock = value;
        }
    }

    private void Awake()
    {
        thisCollider = GetComponent<Collider>();
        center = transform.position;
    }

    public void OnPlaced()
    {
        if (RandomSize) RandomizeSize();
        // debug
        StartCoroutine(DebugPop(2));
    }

    private void RandomizeSize()
    {
        var random = Random.Range(SizeRangeMin, SizeRangeMax);
        Size = random;
        transform.localScale = Vector3.one * Size;
    }

    #region Interactions
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Popper") return;
        popDistance = Size * PopDistanceScale;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Popper") return;
        var distance = Vector3.Distance(other.transform.position, transform.position);
        if (Application.isPlaying) {
            if (distance < popDistance) Pop();
        }
        UpdateMaterial(other.gameObject);
    }

    private void UpdateMaterial(GameObject other)
    {
        _renderer.GetPropertyBlock(_block);
        _block.SetVector("_CollisionPoint", other.transform.position);
        _renderer.SetPropertyBlock(_block);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Popper") return;
        //
    }

    private void Pop()
    {
        // notify all listeners that it was popped
        EventManager.TriggerEvent(Events.EVENT_POPPED);
        // put this object back into the object pool
        PoppablesManager.Instance.ReturnPoppable(this);
    }
    #endregion

    #region DEBUG
    IEnumerator DebugPop(float t)
    {
        yield return new WaitForSeconds(t);
        Pop();
    }
    #endregion
}
