using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData : MonoBehaviour
{
    public static float NODE_MAX_HEIGHT = 20.0f;

    // OCEAN, BEACH, PLAINS, HILL, CLIFF, MOUNTAIN_TOP, LAKE, DESERT, RAIN_FOREST
    public static Dictionary<CommonEnums.NODE_BIOME, float> m_biomeTravelCost = new Dictionary<CommonEnums.NODE_BIOME, float> { 
        { CommonEnums.NODE_BIOME.OCEAN, 20.0f }, { CommonEnums.NODE_BIOME.BEACH, 2.0f }, { CommonEnums.NODE_BIOME.PLAINS, 1.0f }, 
        { CommonEnums.NODE_BIOME.HILL, 1.6f }, { CommonEnums.NODE_BIOME.CLIFF, 100.0f }, { CommonEnums.NODE_BIOME.MOUNTAIN_TOP, 4.0f }, 
        { CommonEnums.NODE_BIOME.LAKE, float.PositiveInfinity }, { CommonEnums.NODE_BIOME.DESERT, 3.0f }, { CommonEnums.NODE_BIOME.RAIN_FOREST, 4.0f }};

    public static Dictionary<CommonEnums.NODE_BIOME, Color> m_nodeTypeColor = new Dictionary<CommonEnums.NODE_BIOME, Color> {
        { CommonEnums.NODE_BIOME.OCEAN, Color.blue }, { CommonEnums.NODE_BIOME.BEACH, Color.yellow }, { CommonEnums.NODE_BIOME.PLAINS, Color.green },
        { CommonEnums.NODE_BIOME.HILL, new Color(.2f, 1.0f, 0.2f)}, { CommonEnums.NODE_BIOME.CLIFF, Color.grey }, { CommonEnums.NODE_BIOME.MOUNTAIN_TOP, Color.white },
        { CommonEnums.NODE_BIOME.LAKE, Color.cyan }, { CommonEnums.NODE_BIOME.DESERT, Color.red }, { CommonEnums.NODE_BIOME.RAIN_FOREST, new Color(.1f, 1.0f, 0.1f)}};

    public static Dictionary<CommonEnums.CARDIANL_DIRECTIONS, Vector2Int> m_cardinalDirection = new Dictionary<CommonEnums.CARDIANL_DIRECTIONS, Vector2Int> 
    {   { CommonEnums.CARDIANL_DIRECTIONS.NORTH, new Vector2Int(0, 1)}, { CommonEnums.CARDIANL_DIRECTIONS.NORTH_EAST, new Vector2Int(1, 1)}, 
        { CommonEnums.CARDIANL_DIRECTIONS.EAST, new Vector2Int(1, 0)}, { CommonEnums.CARDIANL_DIRECTIONS.SOUTH_EAST, new Vector2Int(1, -1)}, 
        { CommonEnums.CARDIANL_DIRECTIONS.SOUTH, new Vector2Int(0, -1)}, { CommonEnums.CARDIANL_DIRECTIONS.SOUTH_WEST, new Vector2Int(-1, -1)},
        { CommonEnums.CARDIANL_DIRECTIONS.WEST, new Vector2Int(-1, 0)}, { CommonEnums.CARDIANL_DIRECTIONS.NORTH_WEST, new Vector2Int(-1, 1)}};

    /// <summary>
    /// Determine node biome using elevation and moisture
    /// </summary>
    /// <param name="p_elevation">Elevation of node, 0.0f-1.0f</param>
    /// <param name="p_moisture">Moisture of node, 0.0f-1.0f</param>
    /// <returns>Biome type</returns>
    public static CommonEnums.NODE_BIOME GetNodeBiome(float p_elevation, float p_moisture)
    {
        if(p_elevation < 0.1f) //Ocean
        {
            return CommonEnums.NODE_BIOME.OCEAN;
        }
        else if (p_elevation < 0.15f)//beach
        {
            return CommonEnums.NODE_BIOME.BEACH;
        }
        else if(p_elevation < 0.4)//level terrain
        {
            if (p_moisture < 0.3f)
                return CommonEnums.NODE_BIOME.DESERT;
            else if (p_moisture < 0.6f)
                return CommonEnums.NODE_BIOME.PLAINS;
            else if (p_moisture < 0.9f)
                return CommonEnums.NODE_BIOME.RAIN_FOREST;
            else
                return CommonEnums.NODE_BIOME.LAKE;

        }
        else if(p_elevation < 0.8)//Hilly terrain
        {
            return CommonEnums.NODE_BIOME.HILL;
        }
        else //Mountain
        {
            return CommonEnums.NODE_BIOME.MOUNTAIN_TOP;
        }
    }
    public static CommonEnums.NODE_TYPE GetNodeType(CommonEnums.NODE_BIOME p_biome)
    {
        return CommonEnums.NODE_TYPE.GROUND;
    }

}
