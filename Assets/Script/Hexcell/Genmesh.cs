using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEditor;

public class Genmesh : MonoBehaviour
{
    public float cell_size;  //格子大小
    public float outline_size;  //边框大小
    public float highlight_size;   //高亮大小
    public Color color; //边框颜色
    public Color color1; //边框颜色
    public Color color2; //边框颜色
    
    Vector3[] cell_vertices = new Vector3[6];    //本顶点
    Vector3[] outline_vertices = new Vector3[6];    //边框顶点
    Vector3[] highlight_vertices = new Vector3[6];  //高亮顶点

    private void Start() {
        Init(cell_vertices, cell_size);
        Init(outline_vertices, cell_size + outline_size);
        Init(highlight_vertices, cell_size + outline_size + highlight_size);
        DrawMesh();
    }

    private void Init(Vector3[] v, float size) {   //对于一个有边框的六边形，其外部的顶点的坐标以及内部顶点的坐标。
        float sq3 = (float)Math.Sqrt(3);
        Vector3[] temp = new Vector3[]{
            new Vector3(sq3/2 * size, .5f * size, 0),
            new Vector3(0, size, 0),
            new Vector3(-sq3/2 * size, .5f * size, 0),
            new Vector3(-sq3/2 * size, -.5f * size, 0),
            new Vector3(0, -size, 0),
            new Vector3(sq3/2 * size, -.5f * size, 0),
        };

        for(int i=0; i<6; i++){
            v[i] = temp[i];
        }
    }

    private void DrawCell(Mesh mesh){
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for(int i=0;i<6;i++){   //添加顶点，内顶点需要不同颜色添加两次
            vertices.Add(cell_vertices[i]);
        }

        vertices.Add(new Vector3(0,0,0));

        for(int i=0;i<6;i++){
            triangles.Add(i%6);
            triangles.Add((i+1)%6);
            triangles.Add(6);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();   //将所有的顶点和三角形赋值到mesh上。

        Vector2[] temp1 = new Vector2[mesh.vertices.Length];
        for(int i=0; i<temp1.Length; i++){
            temp1[i] = new Vector2(mesh.vertices[i].x/(2*cell_size) + .5f, mesh.vertices[i].y/(2*cell_size) + .5f);
            print(temp1[i]);
        }
        mesh.uv = temp1;

        Color[] temp2 = new Color[mesh.vertices.Length];
        for(int i=0; i<temp2.Length; i++){
            temp2[i] = color;
        }
        mesh.colors = temp2;
    }

    private void DrawOut(Mesh mesh, Vector3[] v1, Vector3[] v2, float size, Color color){
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for(int i=0;i<6;i++){   //添加顶点，内顶点需要不同颜色添加两次
            vertices.Add(v1[i]);
        }

        for(int i=0;i<6;i++){   //添加顶点，内顶点需要不同颜色添加两次
            vertices.Add(v2[i]);
        }

        for(int i=0;i<6;i++){   //画出三个三角形，前两个是构成梯形
            triangles.Add(i%6);
            triangles.Add((i+1)%6);
            triangles.Add(i%6 + 6);
            triangles.Add(i%6 + 6);
            triangles.Add((i+1)%6);
            triangles.Add((i+1)%6 + 6);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();   //将所有的顶点和三角形赋值到mesh上。

        Vector2[] temp1 = new Vector2[mesh.vertices.Length];
        for(int i=0; i<temp1.Length; i++){
            temp1[i] = new Vector2(mesh.vertices[i].x/(2*size) + .5f, mesh.vertices[i].y/(2*size) + .5f);
        }

        mesh.uv = temp1;

        Color[] temp2 = new Color[mesh.vertices.Length];
        for(int i=0; i<temp2.Length; i++){
            temp2[i] = color;
        }
        mesh.colors = temp2;
    }

    private void DrawMesh() {
        Mesh cell_mesh = new Mesh(), outline_mesh = new Mesh(), highlight_mesh = new Mesh();
        cell_mesh.name = "cell_mesh";
        outline_mesh.name = "outline_mesh";
        highlight_mesh.name = "highlight_mesh";

        DrawCell(cell_mesh);
        DrawOut(outline_mesh, cell_vertices, outline_vertices, cell_size + outline_size, color1);
        DrawOut(highlight_mesh, outline_vertices, highlight_vertices, cell_size + outline_size + highlight_size, color2);

        AssetDatabase.CreateAsset(cell_mesh , "Assets/Prefab/Cell/cell_mesh.asset");
        AssetDatabase.CreateAsset(outline_mesh , "Assets/Prefab/Cell/outline_mesh.asset");
        AssetDatabase.CreateAsset(highlight_mesh , "Assets/Prefab/Cell/highlight_mesh.asset");
    }
}
