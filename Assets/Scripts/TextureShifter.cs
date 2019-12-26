using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureShifter : MonoBehaviour
{
    public Vector2 ShiftDirection;
    public float ShiftSpeed;
    public float TextureScale = 1;

    public Texture2D MainTexture;
    public Color Tint;

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
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        _renderer.GetPropertyBlock(_block);
        _block.SetTexture("_MainTex", MainTexture != null ? MainTexture : Texture2D.whiteTexture);
        _block.SetColor("_Color", Tint);
        _block.SetFloat("_Scale", TextureScale);
        _renderer.SetPropertyBlock(_block);

    }

    private void OnValidate()
    {
        UpdateMaterial();
    }
}
