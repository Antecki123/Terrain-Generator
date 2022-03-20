using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Texture2D[] texturesAtlas;
    public static Dictionary<string, Rect> atlasDictionary = new Dictionary<string, Rect>();
    public static List<BlockType> blockTypes = new List<BlockType>();

    public static int chunkSize = 8;
    public static Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();

    [SerializeField]
    private int worldSize = 16;
    private Material blockMaterial;

    private void Start()
    {
        Texture2D atlas = GetTextureAtlas();
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.mainTexture = atlas;
        blockMaterial = material;

        GenerateBlockTypes();
        ChunkUtils.GenerateRandomOffset();

        GenerateWorld();
        StartCoroutine(BuildWorld());
    }

    /*
    public void DestroyBlocks(string chunkName, Vector3 blockPosition)
    {
        Chunk chunk;

        if (chunks.TryGetValue(chunkName, out chunk))
        {
            chunk.DestroyBlock(blockPosition);
        }


    }*/

    private void GenerateBlockTypes()
    {
        BlockType air = new BlockType("air", true);
        air.UV = SetBlockTypeUV("air");
        air.GenerateBlockUVs();
        blockTypes.Add(air);

        BlockType grass = new BlockType("grass", false);
        grass.UV = SetBlockTypeUV("grass");
        grass.GenerateBlockUVs();
        blockTypes.Add(grass);

        BlockType dirt = new BlockType("dirt", false);
        dirt.UV = SetBlockTypeUV("dirt");
        dirt.GenerateBlockUVs();
        blockTypes.Add(dirt);

        BlockType stone = new BlockType("stone", false);
        stone.UV = SetBlockTypeUV("stone");
        stone.GenerateBlockUVs();
        blockTypes.Add(stone);

        BlockType sand = new BlockType("sand", false);
        sand.UV = SetBlockTypeUV("sand");
        sand.GenerateBlockUVs();
        blockTypes.Add(sand);
    }

    private void GenerateWorld()
    {
        for (int z = 0; z < worldSize; z++)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    string chunkName = (int)chunkPosition.x + "_" + (int)chunkPosition.y + "_" + (int)chunkPosition.z;

                    Chunk chunk = new Chunk(chunkName, chunkPosition, blockMaterial);
                    chunk.chunkObject.transform.parent = transform;
                    chunks.Add(chunkName, chunk);

                    chunk.chunkObject.isStatic = true;
                }
            }
        }
    }

    // Generate chunks collumn
    IEnumerator BuildWorld()
    {
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            chunk.Value.DrawChunk(chunkSize);
            yield return null;
        }
    }

    private Vector2[] SetBlockTypeUV(string name)
    {
        if (name == "air")
        {
            return new Vector2[4] { new Vector2(0f, 0f),
                                    new Vector2(1f, 0f),
                                    new Vector2(0f, 1f),
                                    new Vector2(1f, 1f)};
        }
        else
        {
            return new Vector2[4] { new Vector2(atlasDictionary[name].x,
                                                atlasDictionary[name].y),

                                    new Vector2(atlasDictionary[name].x + atlasDictionary[name].width,
                                                atlasDictionary[name].y),
                                 
                                    new Vector2(atlasDictionary[name].x,
                                                atlasDictionary[name].y + atlasDictionary[name].height),
                                 
                                    new Vector2(atlasDictionary[name].x + atlasDictionary[name].width,
                                                atlasDictionary[name].y + atlasDictionary[name].height)};
        }
    }

    // Generate texture atlas
    private Texture2D GetTextureAtlas()
    {
        int textureSize = 8192;
        Texture2D textureAtlas = new Texture2D(textureSize, textureSize);
        Rect[] rectCoordinates = textureAtlas.PackTextures(texturesAtlas, 0, textureSize, false);
        textureAtlas.Apply();

        for (int i = 0; i < rectCoordinates.Length; i++)
        {
            atlasDictionary.Add(texturesAtlas[i].name.ToLower(), rectCoordinates[i]);
        }
        return textureAtlas;
    }
}
