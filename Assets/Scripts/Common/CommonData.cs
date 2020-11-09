using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData : MonoBehaviour
{
    public static Dictionary<CommonEnums.NODE_TYPE, float> m_nodeTypeCost = new Dictionary<CommonEnums.NODE_TYPE, float> { { CommonEnums.NODE_TYPE.GRASS, 1.0f }, { CommonEnums.NODE_TYPE.STONE, 1.6f }, { CommonEnums.NODE_TYPE.BUILDING, float.PositiveInfinity } };

    public static Dictionary<CommonEnums.CARDIANL_DIRECTIONS, Vector2Int> m_cardinalDirection = new Dictionary<CommonEnums.CARDIANL_DIRECTIONS, Vector2Int> 
    {   { CommonEnums.CARDIANL_DIRECTIONS.NORTH, new Vector2Int(0, 1)}, { CommonEnums.CARDIANL_DIRECTIONS.NORTH_EAST, new Vector2Int(1, 1)}, 
        { CommonEnums.CARDIANL_DIRECTIONS.EAST, new Vector2Int(1, 0)}, { CommonEnums.CARDIANL_DIRECTIONS.SOUTH_EAST, new Vector2Int(1, -1)}, 
        { CommonEnums.CARDIANL_DIRECTIONS.SOUTH, new Vector2Int(0, -1)}, { CommonEnums.CARDIANL_DIRECTIONS.SOUTH_WEST, new Vector2Int(-1, -1)},
        { CommonEnums.CARDIANL_DIRECTIONS.WEST, new Vector2Int(-1, 0)}, { CommonEnums.CARDIANL_DIRECTIONS.NORTH_WEST, new Vector2Int(-1, 1)}};

}
