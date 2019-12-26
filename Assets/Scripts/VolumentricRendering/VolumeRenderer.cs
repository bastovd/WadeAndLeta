using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VolumeRenderer : MonoBehaviour
{
    [Header("Particles Test")]
    public ParticleSystem system;
    private ParticleSystem.Particle[] particles;

    public Material material
    {
        get
        {
            if (Application.isPlaying)
            {
                return GetComponent<MeshRenderer>().material;
            } else
            {
                return GetComponent<MeshRenderer>().sharedMaterial;
            }

        }
    }

    [System.Serializable]
    public struct VolumetricProps
    {
        public Vector3 center;
        public float radius;
    }
    public List<VolumetricProps> objects;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateProps();
        }
    }

    //private ParticleSystem.Particle[] particles;

    private void UpdateProps()
    {
        if (objects != null)
        {
            material.SetInt("_Count", objects.Count);
            if (objects.Count >= 2)
            {
                var centers = new List<Vector4>();//[objects.Count];
                var radii = new List<float>();//[objects.Count];
                for (int i = 0; i < objects.Count; ++i)
                {
                    centers.Add(objects[i].center);
                    radii.Add(objects[i].radius);
                }
                material.SetVectorArray("_Centers", centers);
                material.SetFloatArray("_Radii", radii);
            }
        }
    }

    private void Awake()
    {
        if (system == null)
        {
            system = GetComponent<ParticleSystem>();
        }
        particles = new ParticleSystem.Particle[system.main.maxParticles];
        material.SetInt("_Count", particles.Length);

        //if (material == null)
        //{
        //    material = GetComponent<MeshRenderer>().material;
        //}
    }

    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            var count = system.GetParticles(particles);
            if (count < particles.Length) return;

            var centers = new List<Vector4>();
            var radii = new List<float>();
            for (int i = 0; i < count; ++i)
            {
                centers.Add(particles[i].position);
                radii.Add(particles[i].GetCurrentSize(system));
            }
            material.SetVectorArray("_Centers", centers);
            material.SetFloatArray("_Radii", radii);

        }
    }
}
