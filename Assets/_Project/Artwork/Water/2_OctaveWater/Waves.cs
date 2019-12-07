using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Waves : MonoBehaviour
{
    [SerializeField]
    private int dimension = 10;
    [SerializeField]
    private Vector2 uvScale = Vector2.one;
    [SerializeField]
    private Octave[] octaves = new Octave[1];

    protected Mesh mesh;
    protected MeshFilter meshFilter;

    private int TotalVertices => (dimension + 1) * (dimension + 1);
    private int Index(int x, int z) => x * (dimension + 1) + z;

    [Serializable]
    private class Octave
    {
        public Vector2 speed = Vector2.one;
        public Vector2 scale = Vector2.one;
        public float height = 1f;
        public bool alternate = true;
    }

    private void Awake()
    {
        mesh = new Mesh
        {
            name = $"{gameObject.name}Mesh",
            vertices = GenerateVertices(),
            triangles = GenerateTriangles(),
            uv = GenerateUVs(),
        };
        mesh.RecalculateBounds();

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private Vector3[] GenerateVertices()
    {
        var vertices = new Vector3[TotalVertices];

        // Equally distribute vertices
        for(int x = 0; x <= dimension; ++x)
            for(int z = 0; z <= dimension; ++z)
                vertices[Index(x, z)] = new Vector3(x, 0, z);

        return vertices;
    }

    private int[] GenerateTriangles()
    {
        var triangles = new int[TotalVertices * 6];

        // Two triangles for one tile
        for(int x = 0; x < dimension; ++x)
        {
            for(int z = 0; z < dimension; ++z)
            {
                int index = Index(x, z) * 6;
                triangles[index + 0] = Index(x, z);
                triangles[index + 1] = Index(x + 1, z + 1);
                triangles[index + 2] = Index(x + 1, z);
                triangles[index + 3] = Index(x, z);
                triangles[index + 4] = Index(x, z + 1);
                triangles[index + 5] = Index(x + 1, z + 1);
            }
        }

        return triangles;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[TotalVertices];

        // Always set one uv over n tiles than flip the uv and set it again
        for(int x = 0; x <= dimension; ++x)
        {
            for(int z = 0; z <= dimension; ++z)
            {
                var vector = new Vector2(x, z) / uvScale;
                vector.x %= 2;
                vector.y %= 2;
                uvs[Index(x, z)] = new Vector2(vector.x <= 1 ? vector.x : 2 - vector.x, vector.y <= 1 ? vector.y : 2 - vector.y);
            }
        }

        return uvs;
    }


    private void Update()
    {
        UpdateVertices();
    }

    private void UpdateVertices()
    {
        var vertices = mesh.vertices;

        for(int x = 0; x <= dimension; ++x)
            for(int z = 0; z <= dimension; ++z)
            {
                float y = 0f;
                for(int o = 0; o < octaves.Length; ++o)
                {
                    Octave octave = octaves[o];
                    if(octave.alternate)
                    {
                        Vector2 noisePos = new Vector2(x, z) * octave.scale / dimension;
                        float noise = Mathf.PerlinNoise(noisePos.x, noisePos.y) * Mathf.PI * 2;
                        y += Mathf.Cos(noise + octave.speed.magnitude * Time.time) * octave.height;
                    }
                    else
                    {
                        Vector2 noisePos = (new Vector2(x, z) * octave.scale + Time.time * octave.speed) / dimension;
                        float noise = Mathf.PerlinNoise(noisePos.x, noisePos.y) - 0.5f;
                        y = noise * octave.height;
                    }
                }

                vertices[Index(x, z)] = new Vector3(x, y, z);
            }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    public float Height(Vector3 position)
    {
        // Scale factor and position in local space
        var scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
        var localPos = Vector3.Scale(position - transform.position, scale);

        // Get edge points
        var p1 = Point(Mathf.Floor(localPos.x), Mathf.Floor(localPos.z));
        var p2 = Point(Mathf.Floor(localPos.x), Mathf.Ceil(localPos.z));
        var p3 = Point(Mathf.Ceil(localPos.x), Mathf.Floor(localPos.z));
        var p4 = Point(Mathf.Ceil(localPos.x), Mathf.Ceil(localPos.z));

        Vector3 Point(float x, float z) => new Vector3
        {
            x = Mathf.Clamp(x, 0, dimension),
            z = Mathf.Clamp(z, 0, dimension),
        };

        var p1Dist = Vector3.Distance(p1, localPos);
        var p2Dist = Vector3.Distance(p2, localPos);
        var p3Dist = Vector3.Distance(p3, localPos);
        var p4Dist = Vector3.Distance(p4, localPos);

        // Get the max distance to one of the edges and take that to compute max - dist
        var max = Mathf.Max(p1Dist, p2Dist, p3Dist, p4Dist + Mathf.Epsilon);
        var distance = (max - p1Dist)
            + (max - p2Dist)
            + (max - p3Dist)
            + (max - p4Dist + Mathf.Epsilon);

        // Weighted sum
        var height = mesh.vertices[Index((int)p1.x, (int)p1.z)].y * (max - p1Dist)
            + mesh.vertices[Index((int)p2.x, (int)p2.z)].y * (max - p2Dist)
            + mesh.vertices[Index((int)p3.x, (int)p3.z)].y * (max - p3Dist)
            + mesh.vertices[Index((int)p4.x, (int)p4.z)].y * (max - p4Dist);

        // Scale
        return height * transform.lossyScale.y / distance;
    }
}
