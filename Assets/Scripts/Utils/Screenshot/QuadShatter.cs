using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class QuadShatter : MonoBehaviour
{
    public Transform triangleContainer;
    private List<(Rigidbody,Vector3)> rbs = new List<(Rigidbody, Vector3)>();
    public static QuadShatter Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public void Shatter(MeshRenderer quad)
    {
        Camera.main.cullingMask = 1 << 9;
        Camera.main.backgroundColor = Color.black;
        quad.gameObject.layer = 9;
        var points = new List<Vector3[]>()
        {
            new Vector3[3] { new Vector3(-2, 0, 0), new Vector3(2, 2.5f, 0), new Vector3(1.5f, -2, 0) },
            new Vector3[3] { new Vector3(-8, 4.5f, 0), new Vector3(0, 4.5f, 0), new Vector3(-2, 0, 0) },
            new Vector3[3] { new Vector3(-2, 0, 0), new Vector3(0, 4.5f, 0), new Vector3(2, 2.5f, 0) },
            new Vector3[3] { new Vector3(2, 2.5f, 0), new Vector3(0, 4.5f, 0), new Vector3(8, 4.5f, 0) },
            new Vector3[3] { new Vector3(2, 2.5f, 0), new Vector3(8, 4.5f, 0), new Vector3(8, -4.5f, 0) },
            new Vector3[3] { new Vector3(2, 2.5f, 0), new Vector3(8, -4.5f, 0), new Vector3(1.5f, -2f, 0) },
            new Vector3[3] { new Vector3(1.5f, -2, 0), new Vector3(8, -4.5f, 0), new Vector3(0, -4.5f, 0) },
            new Vector3[3] { new Vector3(-2, 0, 0), new Vector3(1.5f, -2, 0), new Vector3(0, -4.5f, 0) },
            new Vector3[3] { new Vector3(-2, 0, 0), new Vector3(0, -4.5f, 0), new Vector3(-8, -4.5f, 0) },
            new Vector3[3] { new Vector3(-8, 4.5f, 0), new Vector3(-2, 0, 0), new Vector3(-8, -4.5f, 0) },
        };

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points[i].Length; j++)
            {
                points[i][j].x /= 16f;
                points[i][j].y /= 9f;
            }

        }

        Sequence sequence = DOTween.Sequence();
        int[] splashCount = new int[3] { 3, 3, 4 };
        int cnt = 0;
        for (int i = 0; i < splashCount.Length; i ++) 
        {
            var p = i;
            for (int j = 0; j < splashCount[i]; j++) 
            {
                sequence.AppendCallback(() =>
                {
                    CreateTriangle(quad, points[cnt]);
                    if (p == 0)
                        AudioManager.Instance.PlayOneShot(SFXNAME.Sword);
                    else if (p == 1)
                        AudioManager.Instance.PlayOneShot(SFXNAME.Sword2);
                    else if (p == 2)
                        AudioManager.Instance.PlayOneShot(SFXNAME.Sword3);
                    cnt++;
                });
                
            }
            sequence.AppendInterval(1f);
        }
        //quad.gameObject.SetActive(false);
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() =>
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.ScreenShatter);
            quad.gameObject.SetActive(false);
            foreach (var r in rbs)
            {
                r.Item1.useGravity = true;
                r.Item1.AddForce(r.Item2 * Random.Range(1000, 1500));
                
            }
        });
        sequence.AppendInterval(2f);
        sequence.AppendCallback(()=>SceneTransition.Instance.FadeToBlack());
    }
    public GameObject CreateTriangle(MeshRenderer input, Vector3[] vertices)
    {
        GameObject triangle = new GameObject("Triangle");
        MeshFilter meshFilter = triangle.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = triangle.AddComponent<MeshRenderer>();

        triangle.transform.position = input.transform.position;

        Mesh mesh = new Mesh();
        int[] triangles = new int[3] { 0, 1, 2 };  
        Vector2[] uv = new Vector2[3];
        for(int i = 0; i < 3; i++)
        {
            uv[i].x = vertices[i].x + 0.5f;
            uv[i].y = vertices[i].y + 0.5f;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        meshFilter.mesh = mesh;

        triangle.transform.localScale = input.transform.localScale;
        triangle.transform.parent = triangleContainer;
        triangle.layer = 9;

        Rigidbody rigidbody = triangle.AddComponent<Rigidbody>();
        Vector3 center = vertices[0] + vertices[1] + vertices[2];
        center.Normalize();
        rbs.Add((rigidbody, center));
        rigidbody.useGravity = false;
        //rigidbody.AddForce(center * Random.Range(1000, 1500));


        //Material
        Vector3[] worldPositions = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            worldPositions[i].x = vertices[i].x * 16;
            worldPositions[i].y = vertices[i].y * 9;
            worldPositions[i].z = 0;
        }
        var material = new Material(input.material);
        material.SetVector("_Vertex1", worldPositions[0]);
        material.SetVector("_Vertex2", worldPositions[1]);
        material.SetVector("_Vertex3", worldPositions[2]);
        material.SetFloat("_FadeWidth", 0.2f);
        mesh.SetUVs(1, worldPositions);
        meshRenderer.material = material;

        var pos = meshRenderer.transform.position;
        pos.z = 0;
        meshRenderer.transform.position = pos;
        var newPos = pos + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
        meshRenderer.transform.DOMove(newPos, 0.1f);
        return triangle;
    }
}
