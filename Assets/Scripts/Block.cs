using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum BLOCKSIDE { Front, Back, Left, Right, Top, Bottom }

    private BlockType blockType;
    private Chunk chunkParent;
    private bool isTransparent;

    private GameObject blockParent;
    private Vector3 blockPosition;

    private static int[] triangles = new int[] { 3, 1, 0, 3, 2, 1 };

    private static Vector3[] vertices = new Vector3[8] { new Vector3(-0.5f, -0.5f,  0.5f),
                                                         new Vector3( 0.5f, -0.5f,  0.5f),
                                                         new Vector3( 0.5f, -0.5f, -0.5f),
                                                         new Vector3(-0.5f, -0.5f, -0.5f),
                                                         new Vector3(-0.5f,  0.5f,  0.5f),
                                                         new Vector3( 0.5f,  0.5f,  0.5f),
                                                         new Vector3( 0.5f,  0.5f, -0.5f),
                                                         new Vector3(-0.5f,  0.5f, -0.5f)};


    private static Vector3[] frontVertices = new Vector3[] { vertices[4], vertices[5], vertices[1], vertices[0] };
    private static Vector3[] backVertices = new Vector3[] { vertices[6], vertices[7], vertices[3], vertices[2] };
    private static Vector3[] leftVertices = new Vector3[] { vertices[7], vertices[4], vertices[0], vertices[3] };
    private static Vector3[] rightVertices = new Vector3[] { vertices[5], vertices[6], vertices[2], vertices[1] };
    private static Vector3[] upVertices = new Vector3[] { vertices[7], vertices[6], vertices[5], vertices[4] };
    private static Vector3[] downVertices = new Vector3[] { vertices[0], vertices[1], vertices[2], vertices[3] };


    public Block(BlockType type, Chunk parent, Vector3 position)
    {
        this.blockType = type;
        this.chunkParent = parent;
        this.blockPosition = position;
        this.blockParent = parent.chunkObject;

        if (blockType.isTransparent)         // || blockType == BlockType.Water
            isTransparent = true;
        else
            isTransparent = false;

    }

    public void CreateBlock()
    {
        if (blockType.isTransparent)
            return;

        if (TransparentNeighbour(BLOCKSIDE.Front))
            GenerateBlockSide(BLOCKSIDE.Front);

        if (TransparentNeighbour(BLOCKSIDE.Back))
            GenerateBlockSide(BLOCKSIDE.Back);

        if (TransparentNeighbour(BLOCKSIDE.Left))
            GenerateBlockSide(BLOCKSIDE.Left);

        if (TransparentNeighbour(BLOCKSIDE.Right))
            GenerateBlockSide(BLOCKSIDE.Right);

        if (TransparentNeighbour(BLOCKSIDE.Top))
            GenerateBlockSide(BLOCKSIDE.Top);

        if (TransparentNeighbour(BLOCKSIDE.Bottom))
            GenerateBlockSide(BLOCKSIDE.Bottom);
    }

    // Check transparency of the neighbour block
    bool TransparentNeighbour(BLOCKSIDE blockSide)
    {
        Block[,,] chunkBlocs = chunkParent.chunkBlocks;
        Vector3 neighbourPosition = new Vector3();

        if (blockSide == BLOCKSIDE.Front)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z + 1);
        else if (blockSide == BLOCKSIDE.Back)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z - 1);
        else if (blockSide == BLOCKSIDE.Top)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z);
        else if (blockSide == BLOCKSIDE.Bottom)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y - 1, blockPosition.z);
        else if (blockSide == BLOCKSIDE.Right)
            neighbourPosition = new Vector3(blockPosition.x + 1, blockPosition.y, blockPosition.z);
        else if (blockSide == BLOCKSIDE.Left)
            neighbourPosition = new Vector3(blockPosition.x - 1, blockPosition.y, blockPosition.z);

        if (neighbourPosition.x >= 0 && neighbourPosition.x < chunkBlocs.GetLength(0) &&
            neighbourPosition.y >= 0 && neighbourPosition.y < chunkBlocs.GetLength(1) &&
            neighbourPosition.z >= 0 && neighbourPosition.z < chunkBlocs.GetLength(2))
        {
            return chunkBlocs[(int)neighbourPosition.x, (int)neighbourPosition.y, (int)neighbourPosition.z].isTransparent;
        }
        return true;
    }

    // Generating mesh values according to the side of block
    private void GenerateBlockSide(BLOCKSIDE side)
    {
        switch (side)
        {
            case BLOCKSIDE.Front:
                foreach (Vector3 vertex in frontVertices)
                {
                    chunkParent.vertices.Add(blockPosition + vertex);
                }
                break;

            case BLOCKSIDE.Back:
                foreach (Vector3 vertex in backVertices)
                {
                    chunkParent.vertices.Add(blockPosition + vertex);
                }
                break;

            case BLOCKSIDE.Left:
                foreach (Vector3 vertex in leftVertices)
                {
                    chunkParent.vertices.Add(blockPosition + vertex);
                }
                break;

            case BLOCKSIDE.Right:
                foreach (Vector3 vertex in rightVertices)
                {
                    chunkParent.vertices.Add(blockPosition + vertex);
                }
                break;

            case BLOCKSIDE.Top:
                foreach (Vector3 vertex in upVertices)
                {
                    chunkParent.vertices.Add(blockPosition + vertex);
                }
                break;

            case BLOCKSIDE.Bottom:
                foreach (Vector3 vertex in downVertices)
                {
                    chunkParent.vertices.Add(blockPosition + vertex);
                }
                break;
        }

        foreach (Vector2 blockUV in blockType.GetBlockUVs(side))
        {
            chunkParent.uvs.Add(blockUV);
        }

        foreach (int triangle in triangles)
        {
            chunkParent.triangles.Add(chunkParent.vertexIndex + triangle);
        }

        chunkParent.vertexIndex += 4;
    }
}

public class BlockType
{
    public string name { get; private set; }
    public bool isTransparent { get; private set; }
    public Vector2[] UV { get; set; }

    private List<Vector2[]> blockUVs = new List<Vector2[]>();

    public BlockType(string name, bool isTransparent)
    {
        this.name = name;
        this.isTransparent = isTransparent;
    }

    public void GenerateBlockUVs() => blockUVs.Add(new Vector2[] { UV[3], UV[2], UV[0], UV[1] });
    public Vector2[] GetBlockUVs(Block.BLOCKSIDE blockSide)
    {
        return blockUVs[0];
    }
}