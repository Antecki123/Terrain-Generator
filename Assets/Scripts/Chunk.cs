using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Block[,,] chunkBlocks;
    public GameObject chunkObject;

    public int vertexIndex { get; set; }
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();

    private Material blockMaterial;

    public Chunk(string name, Vector3 position, Material material)
    {
        this.chunkObject = new GameObject(name);
        this.chunkObject.transform.position = position;
        this.blockMaterial = material;

        GenerateChunk(World.chunkSize);
    }

    private void GenerateChunk(int chunkSize)
    {
        chunkBlocks = new Block[chunkSize, chunkSize, chunkSize];

        for (int z = 0; z < chunkSize; z++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float worldX = x + chunkObject.transform.position.x;
                    float worldY = y + chunkObject.transform.position.y;
                    float worldZ = z + chunkObject.transform.position.z;

                    int surfaceLevel = (int)ChunkUtils.GenerateSurfaceHeight(worldX, worldZ);
                    int dirtLevel = (int)ChunkUtils.GenerateDirtHeight(worldX, worldZ, surfaceLevel);

                    if (worldY == surfaceLevel)          //grass level
                        chunkBlocks[x, y, z] = new Block((World.blockTypes[1]), this, new Vector3(x, y, z));

                    else if (worldY < dirtLevel)         //stone level
                        chunkBlocks[x, y, z] = new Block((World.blockTypes[3]), this, new Vector3(x, y, z));

                    else if (worldY < surfaceLevel)      //dirt level
                        chunkBlocks[x, y, z] = new Block((World.blockTypes[2]), this, new Vector3(x, y, z));

                    else
                        chunkBlocks[x, y, z] = new Block((World.blockTypes[0]), this, new Vector3(x, y, z));

                }
            }
        }
    }

    public void DrawChunk(int chunkSize)
    {
        vertexIndex = 0;

        for (int z = 0; z < chunkSize; z++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    chunkBlocks[x, y, z].CreateBlock();
                }
            }
        }
        MergeSides();
    }

    private void MergeSides()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter blockMeshFilter = chunkObject.AddComponent<MeshFilter>();
        blockMeshFilter.mesh = mesh;

        MeshRenderer blockMeshRenderer = chunkObject.AddComponent<MeshRenderer>();
        blockMeshRenderer.material = blockMaterial;

        MeshCollider blockMeshCollider = chunkObject.AddComponent<MeshCollider>();

        foreach (Transform side in chunkObject.transform)
        {
            GameObject.Destroy(side.gameObject);
        }
    }
}