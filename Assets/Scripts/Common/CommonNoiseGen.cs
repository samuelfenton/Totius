using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonNoiseGen : MonoBehaviour
{
    public const int SEED = 1337;

    public const float NOISE_SMOOTHNESS = 20.0f;
    public const float NOISE_FREQUENCY = 1.0f / NOISE_SMOOTHNESS;

    public const float NOISE_REDISTRIBUTION_VAL = 2.2f;

    /// <summary>
    /// Get the elevation of a node based off its global position
    /// </summary>
    /// <param name="p_nodeGlobalPosition">Global position of a node</param>
    /// <returns>Elevation of node, range 0.0f-1.0f</returns>
    public static float GetNodeElevation(Vector2Int p_nodeGlobalPosition)
    {
        p_nodeGlobalPosition.x += SEED;
        p_nodeGlobalPosition.y += SEED;

        Vector2 smoothedPosition = new Vector2(p_nodeGlobalPosition.x * NOISE_FREQUENCY, p_nodeGlobalPosition.y * NOISE_FREQUENCY);

        float elevation = Mathf.PerlinNoise(smoothedPosition.x, smoothedPosition.y) +
            0.5f * Mathf.PerlinNoise(2.0f * smoothedPosition.x, 2.0f * smoothedPosition.y) +
            0.25f * Mathf.PerlinNoise(4.0f * smoothedPosition.x, 4.0f * smoothedPosition.y) +
            0.125f * Mathf.PerlinNoise(8.0f * smoothedPosition.x, 8.0f * smoothedPosition.y);

        elevation /= 1.5f; //Crush down because of extra octaves

        return Mathf.Pow(elevation, NOISE_REDISTRIBUTION_VAL);
    }

    /// <summary>
    /// Get the moisture of a node based off its global position
    /// </summary>
    /// <param name="p_nodeGlobalPosition">Global position of a node</param>
    /// <returns>Moisture of node, range 0.0f-1.0f</returns>
    public static float GetNodeMoisture(Vector2Int p_nodeGlobalPosition)
    {
        p_nodeGlobalPosition.x += SEED;
        p_nodeGlobalPosition.y += SEED;

        Vector2 smoothedPosition = new Vector2(p_nodeGlobalPosition.x * NOISE_FREQUENCY, p_nodeGlobalPosition.y * NOISE_FREQUENCY);

        float moisture = Mathf.PerlinNoise(smoothedPosition.x, smoothedPosition.y);

        return Mathf.Pow(moisture, NOISE_REDISTRIBUTION_VAL);
    }
}
