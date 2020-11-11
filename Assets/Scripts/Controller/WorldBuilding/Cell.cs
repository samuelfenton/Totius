using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public const int CELL_SIZE = 64;
    public const int CELL_SIZE_3X = CELL_SIZE * 3;
    public const float CELL_SIZE_HALF = CELL_SIZE/2.0f;

    private Node[,] m_nodes = new Node[CELL_SIZE, CELL_SIZE];

    public Vector2Int m_cellGridPosition = Vector2Int.zero;


    public void Generate(Vector2Int p_cellGridPosition)
    {
        m_cellGridPosition = p_cellGridPosition;

        for (int y_index = 0; y_index < CELL_SIZE; y_index++)
        {
            for (int x_index = 0; x_index < CELL_SIZE; x_index++)
            {
                Node newNode = new Node();

                float elevation = CommonNoiseGen.GetNodeElevation(new Vector2Int(x_index + m_cellGridPosition.x * CELL_SIZE, y_index + m_cellGridPosition.y * CELL_SIZE));
                float moisture = CommonNoiseGen.GetNodeMoisture(new Vector2Int(x_index + m_cellGridPosition.x * CELL_SIZE, y_index + m_cellGridPosition.y * CELL_SIZE));

                newNode.InitNode(new Vector2Int(x_index, y_index), elevation, moisture, this);
                m_nodes[x_index, y_index] = newNode;
            }
        }
    }

    public void LoadFromFile()
    {

    }

    /// <summary>
    /// Get the node in the given position
    /// </summary>
    /// <returns>array of nodes</returns>
    public Node[,] GetAllNodes()
    {
        return m_nodes;
    }

    /// <summary>
    /// Get the node in the given position
    /// </summary>
    /// <param name="p_gridPosition">Position to look for</param>
    /// <returns>Node, null when invalid position</returns>
    public Node GetNode(Vector2Int p_gridPosition)
    {
        if (p_gridPosition.x >= 0 && p_gridPosition.x < CELL_SIZE && p_gridPosition.y >= 0 && p_gridPosition.y < CELL_SIZE)
            return m_nodes[p_gridPosition.x, p_gridPosition.y];
        return null;
    }

    /// <summary>
    /// Get the center position of a cell
    /// </summary>
    /// <param name="p_cellGridPosition">Cells gird position</param>
    /// <returns>Global center positon of cell</returns>
    public static Vector2 GetCellCenter(Vector2Int p_cellGridPosition)
    {
        return new Vector2(p_cellGridPosition.x * CELL_SIZE + CELL_SIZE_HALF, p_cellGridPosition.y * CELL_SIZE + CELL_SIZE_HALF);
    }
}
