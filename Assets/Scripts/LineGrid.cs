using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineGrid : MonoBehaviour
{
    public enum Face { xy, zx, yz };
    public float gridSize = 1f;
    public int size = 8;
    public Color color = Color.white;
    public Face face = Face.xy;
    public bool back = true;
    public float lineThickness = 0.1f;  // ラインの太さ

    private Mesh mesh;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh = ReGrid(mesh);
    }

    Mesh ReGrid(Mesh mesh)
    {
        // 材料設定
        if (back)
        {
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
        else
        {
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("GUI/Text Shader"));
        }

        mesh.Clear();

        int resolution = (size * 2 + 2) * 2;
        Vector3[] vertices = new Vector3[resolution * 4];
        int[] triangles = new int[resolution * 6];
        Vector2[] uvs = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];

        float halfGridSize = gridSize * size / 2.0f;
        float step = gridSize;
        int vertIndex = 0;
        int triIndex = 0;

        for (float i = -halfGridSize; i <= halfGridSize; i += step)
        {
            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            switch (face)
            {
                case Face.xy:
                    start = new Vector3(i, -halfGridSize, 0);
                    end = new Vector3(i, halfGridSize, 0);
                    AddLine(start, end, lineThickness, ref vertIndex, ref triIndex, vertices, triangles, colors);
                    start = new Vector3(-halfGridSize, i, 0);
                    end = new Vector3(halfGridSize, i, 0);
                    AddLine(start, end, lineThickness, ref vertIndex, ref triIndex, vertices, triangles, colors);
                    break;
                case Face.zx:
                    start = new Vector3(i, 0, -halfGridSize);
                    end = new Vector3(i, 0, halfGridSize);
                    AddLine(start, end, lineThickness, ref vertIndex, ref triIndex, vertices, triangles, colors);
                    start = new Vector3(-halfGridSize, 0, i);
                    end = new Vector3(halfGridSize, 0, i);
                    AddLine(start, end, lineThickness, ref vertIndex, ref triIndex, vertices, triangles, colors);
                    break;
                case Face.yz:
                    start = new Vector3(0, i, -halfGridSize);
                    end = new Vector3(0, i, halfGridSize);
                    AddLine(start, end, lineThickness, ref vertIndex, ref triIndex, vertices, triangles, colors);
                    start = new Vector3(0, -halfGridSize, i);
                    end = new Vector3(0, halfGridSize, i);
                    AddLine(start, end, lineThickness, ref vertIndex, ref triIndex, vertices, triangles, colors);
                    break;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;

        return mesh;
    }

    void AddLine(Vector3 start, Vector3 end, float thickness, ref int vertIndex, ref int triIndex, Vector3[] vertices, int[] triangles, Color[] colors)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0) * thickness / 2;
        if (face == Face.zx)
        {
            perpendicular = new Vector3(-direction.z, 0, direction.x) * thickness / 2;
        }
        else if (face == Face.yz)
        {
            perpendicular = new Vector3(0, -direction.z, direction.y) * thickness / 2;
        }

        // 頂点の追加
        vertices[vertIndex] = start - perpendicular;
        vertices[vertIndex + 1] = start + perpendicular;
        vertices[vertIndex + 2] = end + perpendicular;
        vertices[vertIndex + 3] = end - perpendicular;

        // 三角形のインデックス
        triangles[triIndex] = vertIndex;
        triangles[triIndex + 1] = vertIndex + 1;
        triangles[triIndex + 2] = vertIndex + 2;
        triangles[triIndex + 3] = vertIndex;
        triangles[triIndex + 4] = vertIndex + 2;
        triangles[triIndex + 5] = vertIndex + 3;

        // 色の設定
        for (int i = 0; i < 4; i++)
        {
            colors[vertIndex + i] = color;
        }

        vertIndex += 4;
        triIndex += 6;
    }
}
