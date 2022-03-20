using UnityEngine;

public class ChunkUtils
{
    private static int surfaceLevelOffset = 0;
    private static int dirtLevelOffset = 0;

    private static int maxHeight = 40;

    // Propery responsible for the terrain undulating
    private static float increment = .02f;

    public static float GenerateSurfaceHeight(float x, float z)
    {
        float height = Map(1, maxHeight, 0, 1, PerlinNoise(x * increment + surfaceLevelOffset, z * increment + surfaceLevelOffset));
        return height;
    }

    public static float GenerateDirtHeight(float x, float z, int maxHeight)
    {
        float stoneIncrementOffset = 2f;
        float height = Map(1, maxHeight, 0, 1, PerlinNoise(x * increment + dirtLevelOffset, z * increment * stoneIncrementOffset + dirtLevelOffset));
        return height;
    }

    private static float Map(float from, float to, float from2, float to2, float value)
    {
        if (value <= from2)
            return from;
        if (value >= to2)
            return to;

        return (to - from) * ((value - from2) / (to2 - from2)) + from;
    }

    private static float PerlinNoise(float x, float z)
    {
        float height = Mathf.PerlinNoise(x, z);
        return height;
    }

    public static void GenerateRandomOffset()
    {
        surfaceLevelOffset = Random.Range(0, 1000);
        dirtLevelOffset = Random.Range(0, 1000);
    }
}