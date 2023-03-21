using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad 
{
    Planet_Variables planet_variables;
    List<Noise_Value> noise_values;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    private float Min_Height = float.MaxValue;
    private float Max_Height = float.MinValue;
    public Quad(Mesh mesh,Vector3 localup, Planet_Variables pv,List<Noise_Value> nv)
    {
        this.mesh = mesh;
        resolution = pv.resolution;
        localUp = localup;
        noise_values = nv;
        planet_variables = pv;
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localup, axisA);
    }
    public void construct_quad_mesh(float radius)
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;
        int x = 0;
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                Vector2 percent = new Vector2(j, i) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized*radius;
                for (int w = 0; w < noise_values.Count; w++)
                {
                    if (noise_values[w].show)
                    {
                        pointOnUnitSphere *= (noise_value(pointOnUnitCube.normalized, w) + 1);
                        add_value(pointOnUnitSphere.magnitude);
                    }
                }
                vertices[x] = pointOnUnitSphere;
                if(i!=resolution-1&&j!=resolution-1)
                {
                    triangles[triIndex] = x;
                    triangles[triIndex+1] = x+resolution+1;
                    triangles[triIndex+2] = x+resolution;

                    triangles[triIndex+3] = x;
                    triangles[triIndex+4] = x+1;
                    triangles[triIndex+5] = x+resolution+1;

                    triIndex += 6;
                }
                x++;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        set_color(vertices);
        mesh.RecalculateNormals();
    }
 
    private void set_color(Vector3[] vertices)
    {
        Color32[] color = new Color32[vertices.Length];

        for (int i = 0; i < color.Length; i++)
        {
            color[i] = planet_variables.color.Evaluate(Mathf.InverseLerp(Min_Height, Max_Height,vertices[i].magnitude));
        }
        mesh.colors32 = color;
    }
    private float noise_value(Vector3 point,int k)
    {
        switch (noise_values[k].noise_Type)
        {
            case Noise_Type.simplex:
                return simple_noise_value(point,k);
            case Noise_Type.ridged:
                return ridgid_noise_value(point,k);
            default:
                return 0;
        } 
    }
    private float simple_noise_value(Vector3 point, int k)
    {
        Noise noise = new Noise();
        float noise_value = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i=0;i<noise_values[k].octaves;i++)
        {
            float value = noise.Evaluate(point * frequency + noise_values[k].centre);
            noise_value += (value+1)*.5f*amplitude;
            frequency *= noise_values[k].roughness;
            amplitude *= noise_values[k].persistance;
        }
        noise_value = Mathf.Max(0, noise_value- noise_values[k].min_value);
        return noise_value* noise_values[k].strenght;
    }
    private float ridgid_noise_value(Vector3 point, int k)
    {
        Noise noise = new Noise();
        float noise_value = 0;
        float amplitude = 1;
        float frequency = 1;
        float weight = 1;
        for (int i = 0; i < noise_values[k].octaves; i++)
        {
            float value =1-Mathf.Abs(noise.Evaluate(point * frequency + noise_values[k].centre));
            value *= value*weight;
            weight = value;
            noise_value += value*amplitude;
            frequency *= noise_values[k].roughness;
            amplitude *= noise_values[k].persistance;
        }
        noise_value = Mathf.Max(0, noise_value - noise_values[k].min_value) * noise_values[k].strenght;
        return noise_value;
    }
   
    public  void add_value(float value)
    {
        if (value < Min_Height)
            Min_Height = value;
        if (value > Max_Height)
            Max_Height = value;
    }
}

