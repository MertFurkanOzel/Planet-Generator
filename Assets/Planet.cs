using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("PLANET VARIABLES")]
    [SerializeField] public Planet_Variables planet_variables = new Planet_Variables();

    [Header("NOISE VALUES")]
    [SerializeField] List<Noise_Value> noise_value=new List<Noise_Value>();

    [SerializeField,HideInInspector]
    MeshFilter[] meshFilters;
    Quad[] quads;
    Vector3[] quadsides = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
    private void OnValidate()
    {
        Initiliaze();
        generate_mesh();
    }
    private void Update()
    {
        rotate_planet();
    }
    private void Initiliaze()
    {
        if(meshFilters==null||meshFilters.Length==0)
        meshFilters= new MeshFilter[6];
        quads = new Quad[6];
        for (int i = 0; i < 6; i++)
        {
            if(meshFilters[i]==null)
            {
                GameObject meshobj = new GameObject("Mesh");
                meshobj.transform.parent = transform;
                meshobj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
                meshFilters[i] = meshobj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }          
            quads[i] = new Quad(meshFilters[i].sharedMesh,quadsides[i],planet_variables,noise_value);
        }
    }
    private void generate_mesh()
    {
        foreach (var item in quads)
        {
            item.construct_quad_mesh(planet_variables.radius);
        }
    }
    private void  rotate_planet()
    {    
       if(planet_variables.rotate)
       {
            if (!transform.parent)
            {
                GameObject planet_pivot = new GameObject("Pivot");
                transform.parent = planet_pivot.transform;
            }
            transform.parent.Rotate(new Vector3(0, planet_variables.rotate_speed * Time.deltaTime, 0), Space.World);
       }
    }
}
[System.Serializable]
public class Noise_Value
{
    public bool show = true;
    public Noise_Type noise_Type = Noise_Type.simplex;
    [Range(1, 6)]
    public int octaves = 2;
    public float strenght = 1.2f;
    public float roughness = 1.2f;
    public float persistance = 0.5f;
    public Vector3 centre = Vector3.zero;
    public float min_value = 1;
}

[System.Serializable]
public class Planet_Variables
{
    [Range(2, 256)]
    public int resolution = 10;
    public Gradient color = new Gradient();
    [Range(1, 5)] public float radius = 2;
    public bool rotate = true;
    [Range(1, 10)]
    public float rotate_speed = 1;
}
public enum Noise_Type
{
    simplex,
    ridged
}

