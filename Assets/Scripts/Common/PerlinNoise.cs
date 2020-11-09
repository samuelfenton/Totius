using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public const float HEIGHT = 10.0f;
    public const float SMOOTHNESS = 2.0f;

    /// <summary>
    /// Determine height for a node given its position
    /// TODO implement custom noise
    /// </summary>
    /// <param name="p_x">X position of node</param>
    /// <param name="p_z">Z position of node</param>
    /// <param name="p_seed">Seed used for noise gen</param>
    /// <returns>Height determiened by perlin noise</returns>
    public static float GetNodeHeight(int p_x, int p_z, int p_seed)
    {
        float xCoord = p_x / SMOOTHNESS;
        float yCoord = p_z / SMOOTHNESS;
        return HEIGHT * Mathf.PerlinNoise(p_seed + xCoord, p_seed + yCoord);
    }
}
