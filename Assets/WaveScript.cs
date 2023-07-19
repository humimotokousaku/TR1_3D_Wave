using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaveScript : MonoBehaviour
{
    public int resolution = 20;            // メッシュの解像度
    public float amplitudeBigWave = 0.2f;  // 大きい波の高さ
    public float waveVelocity = 6f;        // 速さ
    public float amplitude = 0.2f;         // 小さい波の高さ
    public float T = 1;                    // 周期

    public GameObject cubePrefab;       // 浮かぶCubeのプレハブ

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private float[] waveOffsets;

    private GameObject cube;            // 生成されたCubeの参照

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateWaveOffsets();
        CreateMesh();
        CreateCube();
    }

    void Update()
    {

        UpdateWave();
        UpdateMesh();
        UpdateCube();
    }


    void CreateWaveOffsets()
    {
        waveOffsets = new float[resolution * resolution];
        for (int i = 0; i < waveOffsets.Length; i++)
        {
            waveOffsets[i] = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void CreateMesh()
    {
        int vertexCount = resolution * resolution;
        vertices = new Vector3[vertexCount];

        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        normals = new Vector3[vertexCount];

        int triangleIndex = 0;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int index = x + y * resolution;
                vertices[index] = new Vector3(x, 0f, y);
                uv[index] = new Vector2((float)x / (resolution - 1), (float)y / (resolution - 1));

                if (x < resolution - 1 && y < resolution - 1)
                {
                    int topLeft = index;
                    int topRight = index + 1;
                    int bottomLeft = index + resolution;
                    int bottomRight = index + resolution + 1;

                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.normals = normals;
    }

    void UpdateWave()
    {
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int index = x + y * resolution;
                float waveOffset = waveOffsets[index];
                float waveValue = Mathf.Sin(waveOffset / T * (Time.time - (vertices[index].x / waveVelocity)));
                float bigWave = amplitudeBigWave * Mathf.Sin(2 * Mathf.PI / T * (Time.time - (vertices[index].x / waveVelocity)));

                vertices[index].y = waveValue * amplitude + bigWave;

                normals[index] = Vector3.up;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void CreateCube()
    {
        // Cubeの生成処理...
        cube = Instantiate(cubePrefab, transform.position + Vector3.up * amplitude, Quaternion.identity);
    }

    void UpdateCube()
    {
        Vector3 addPos;
        addPos.x = 20;
        addPos.y = 0;
        addPos.z = -30;
        // Cubeの位置更新処理...
        cube.transform.position = transform.position + Vector3.up * vertices[resolution / 2 + resolution / 2 * resolution].y + addPos;       
    }
}
