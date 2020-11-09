using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnums : MonoBehaviour
{
    public enum CARDIANL_DIRECTIONS {NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST }

    public enum ROT_DIRECTION { CLOCKWISE = 1, ANTI_CLOCKWISE = -1 }

    public enum NODE_TYPE {GRASS, STONE, CLIFF, SHALLOW_WATER, WATER, DEEP_WATER, BUILDING}
}
