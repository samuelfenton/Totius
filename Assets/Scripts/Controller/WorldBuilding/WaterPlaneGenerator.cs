using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlaneGenerator : MonoBehaviour
{
    public const float BASE_PLANE_SIZE = 10.0f;

    private void Start()
    {
        float positionOffset = Cell.CELL_SIZE_HALF;
        float scaleOffset = (Cell.CELL_SIZE * 3.0f) / BASE_PLANE_SIZE;

        transform.localPosition = new Vector3(positionOffset, 0.0f, positionOffset);
        transform.localScale = new Vector3(scaleOffset, 1.0f, scaleOffset);
    }
}
