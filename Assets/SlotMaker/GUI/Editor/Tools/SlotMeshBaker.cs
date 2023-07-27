using UnityEngine;

namespace SlotMaker
{
    public class SlotMeshBaker 
    {
    	public static float BakeCylinderPiece(ref Mesh mesh, int pieceCount, int width, int height, int segmentX, int segmentY)
    	{
    		mesh.Clear();

            float twoPI = 2f * Mathf.PI;
            int segCount = pieceCount * segmentY;
            float radious = ((float)height / (float)segmentY * 0.5f) / Mathf.Sin(0.5f / (float)segCount * twoPI);

    		Vector3[] vertices = new Vector3[(segmentX + 1) * (segmentY + 1)];
            Vector3[] normals = new Vector3[vertices.Length];
            Vector2[] uvs = new Vector2[vertices.Length];
    		for (int y = 0; y < (segmentY + 1); ++y)
    		{
                float rad = ((float)y - ((float)segmentY * 0.5f)) / (float)segCount * twoPI;
                float segmentWidth = (float)width / (float)segmentX;
                Vector3 normal = new Vector3(0f, Mathf.Sin(rad) * radious, -Mathf.Cos(rad) * radious);
                normal.Normalize();

                for (int x = 0; x < (segmentX + 1); ++x)
                {
                    int index = y * (segmentX + 1) + x;
                    vertices[index] = new Vector3(segmentWidth * (float)x - ((float)width * 0.5f), Mathf.Sin(rad) * radious, -Mathf.Cos(rad) * radious);
                    normals[index] = normal;
                    uvs[index] = new Vector2((float)x / (float)segmentX, (float)y / (float)segmentY);
                }
    		}

            int faces = segmentX * segmentY;
            int[] triangles = new int[faces * 6];
            for (int face = 0; face < faces; ++face)
            {
                int p0 = face % segmentX + (face / segmentX * (segmentX + 1));
                int p1 = p0 + 1;
                int p2 = p0 + (segmentX + 1);
                int p3 = p2 + 1;

                int index = face * 6;

                triangles[index]     = p2;
                triangles[index + 1] = p1;
                triangles[index + 2] = p0;

                triangles[index + 3] = p2;
                triangles[index + 4] = p3;
                triangles[index + 5] = p1;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            ;

            return radious;
    	}
    }
}