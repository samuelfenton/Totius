using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public const int CELL_SIZE = 16;

    public Node[,] m_nodes = new Node[CELL_SIZE, CELL_SIZE];

    public Vector2Int m_position = Vector2Int.zero;

    public void Generate(Vector2Int p_postion)
    {
        for (int y_index = 0; y_index < CELL_SIZE; y_index++)
        {
            for (int x_index = 0; x_index < CELL_SIZE; x_index++)
            {
                Node newNode = new Node();
                newNode.InitNode(new Vector2Int(x_index, y_index), PerlinNoise.GetNodeHeight(x_index, y_index, MasterController.SEED), this, CommonEnums.NODE_TYPE.GRASS);
                m_nodes[x_index, y_index] = newNode;
            }
        }
    }

    public void LoadFromFile()
    {

    }
}
